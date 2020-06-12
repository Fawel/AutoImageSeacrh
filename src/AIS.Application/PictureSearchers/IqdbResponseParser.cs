using AIS.Application.Interfaces.Infrastructure;
using AIS.Application.PictureSearchers.Models;
using System;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace AIS.Application.PictureSearchers
{
    public class IqdbResponseParser : IIqdbResponseParser
    {
        const string imageFoundStart = "Best match";
        const string noRelevantMatchesStart = "No relevant matches";
        const string badQueryStart = "Can't read";

        public IqdbSearchResponse ParseResponse(
            StreamReader iqdbResponseStream,
            CancellationToken token = default)
        {
            var bufferFromPool = ArrayPool<char>.Shared.Rent(200);
            var imageFoundStartSpan = imageFoundStart.AsSpan();
            var noRelevantMatchesStartSpan = noRelevantMatchesStart.AsSpan();
            var badQueryStartSpan = badQueryStart.AsSpan();

            IqdbSearchResponse parsedResult = null;

            Span<char> buffer = bufferFromPool;
            int charRead = 0;

            while (!iqdbResponseStream.EndOfStream)
            {
                // записываем сколько мы прочитали
                // это нам пригодится для понимания в какой точке от 
                // начала стрима находится начало блока

                charRead += iqdbResponseStream.Read(buffer);
                var currentIterationSpan = buffer;

                var cantReadQueryBlock = currentIterationSpan.IndexOf(badQueryStartSpan);
                if (cantReadQueryBlock != -1)
                {
                    // значит у нас страница "мы не смогли прочитать ваш файл, идите нафиг"

                    parsedResult = IqdbSearchResponse.Factory.CreateBadQueryResponse();
                    break;
                }

                var noRelevantBlockStart = currentIterationSpan.IndexOf(noRelevantMatchesStartSpan);
                if (noRelevantBlockStart != -1)
                {
                    // значит у нас страница "Не нашли вашу пикчу, но вот вам список похожих"
                    // значит будем парсить как страницу такого типа

                    var noMatchBlock = GetNoMatchBlock(iqdbResponseStream, charRead - buffer.Length + noRelevantBlockStart);
                    var possibleMatchesList = ParsePossibleMatches(noMatchBlock);
                    parsedResult = IqdbSearchResponse.Factory.CreateNotFoundResponse(possibleMatchesList);
                    break;
                }

                var startIndex = currentIterationSpan.IndexOf(imageFoundStartSpan);
                if (startIndex != -1)
                {
                    // это означает, что у нас страница "Нашли вашу пикчу"

                    var pictureFoundBlock = GetPictureFoundBlock(iqdbResponseStream, charRead - buffer.Length + startIndex);
                    var matches = ParsePictureFoundBlock(pictureFoundBlock);
                    parsedResult = IqdbSearchResponse.Factory.CreatePictureFoundResponse(matches[0], matches.Length > 1 ? matches[1..] : new IIqdbImageSearchResult[0]);
                    break;
                }

                // если не нашли ничего - может быть в конце стрима есть начало какого-нибудь 
                // из блоков берём кусочек по максимальному размеру из начал блоков

                var bufferEndLength = Max(imageFoundStartSpan.Length,
                    noRelevantMatchesStartSpan.Length,
                    badQueryStartSpan.Length);
                Span<char> bufferEnd = buffer.Slice(buffer.Length - bufferEndLength);

                // теперь попробуем найти начало каждого блока в этом маленьком кусочке

                var isBlockFound = FindPartOfBlockStarts(iqdbResponseStream, bufferEnd, charRead, out var pars);
                if(isBlockFound)
                {
                    charRead = pars;
                    iqdbResponseStream.BaseStream.Position = pars;
                    iqdbResponseStream.DiscardBufferedData();
                }
            }

            ArrayPool<char>.Shared.Return(bufferFromPool);

            return parsedResult;
        }

        private bool FindPartOfBlockStarts(
            StreamReader iqdbResponseStream,
            ReadOnlySpan<char> bufferEnd,
            int positionBefore,
            out int blockStartPosition,
            Dictionary<string, int> blockStartCharsFound = null)
        {
            blockStartPosition = 0;
            blockStartCharsFound = blockStartCharsFound ?? new Dictionary<string, int>(3)
            {
                [imageFoundStart] = 0,
                [noRelevantMatchesStart] = 0,
                [badQueryStart] = 0
            };

            // делаем копию списка ключей, чтобы не менять исходную коллекцию при итерировании

            var keyCopy = blockStartCharsFound.Keys.Select(x => new string(x)).ToArray();

            static char GetCurrentChar(string blockStart, Dictionary<string, int> dictionary) =>
                blockStart[dictionary[blockStart]];


            // пытаемся найти последовательность букв, частично совпадающих с одним из 
            // начал блоков
            foreach (var spanChar in bufferEnd)
            {
                // берём букву из из спана, берём последовательно текущую букву
                // блока по счётчику. Если совпадают => итерируем счётчик

                foreach (var key in keyCopy)
                {
                    if (blockStartCharsFound[key] == key.Length)
                    {
                        continue;
                    }
                    // если ничего не нашли, то обнуляем счётчики
                    else if (GetCurrentChar(key, blockStartCharsFound) != spanChar)
                    {
                        blockStartCharsFound[key] = 0;
                    }
                    else
                    {
                        blockStartCharsFound[key]++;
                        if (blockStartPosition == 0)
                        {
                            
                        }

                    }


                }

                if (blockStartCharsFound.Values.All(x => x == 0))
                {
                    blockStartPosition = 0;
                }

                //switch (spanChar)
                //{
                //    case var _ when GetCurrentChar(imageFoundStart) == spanChar:
                //        blockStartCharsFound[imageFoundStart]++;
                //        break;
                //    case var _ when GetCurrentChar(noRelevantMatchesStart) == spanChar:
                //        blockStartCharsFound[noRelevantMatchesStart]++;
                //        break;
                //    case var _ when GetCurrentChar(badQueryStart) == spanChar:
                //        blockStartCharsFound[badQueryStart]++;
                //        break;

                //    

                //    default:
                //        foreach (var key in blockStartCharsFound.Keys)
                //        {
                //            blockStartCharsFound[key] = 0;
                //        }
                //        break;
                //}
            }

            // если у нас есть хотя бы один блок целиком, то завершаем выполнение

            bool foundBlockStart = blockStartCharsFound.Any(x => x.Key.Length == x.Value);
            if (foundBlockStart)
            {
                return foundBlockStart;
            }

            // если ни нашли к концу обработки ни одного незавершённого блока, то завершаем поиск

            if (blockStartCharsFound.All(x => x.Value == 0))
            {
                blockStartPosition = 0;
                return foundBlockStart;
            }


            // Если у нас есть хотя бы один блок с необнулённым счётчиком,
            // то предполагаем, что в дальше по стриму есть оставшаяся часть этого блока
            // а значит нам надо чуть-чуть прочитать дальше

            var arrayForBuffer = ArrayPool<char>.Shared.Rent(Max(
                imageFoundStart.Length,
                noRelevantMatchesStart.Length,
                badQueryStart.Length));

            blockStartPosition = positionBefore - blockStartCharsFound.Values.Max();

            Span<char> buffer = arrayForBuffer;
            //while (!iqdbResponseStream.EndOfStream)
            //{
            iqdbResponseStream.Read(buffer);
            foundBlockStart = FindPartOfBlockStarts(
                                iqdbResponseStream,
                                buffer,
                                positionBefore,
                                out _,
                                blockStartCharsFound);
            //}

            ArrayPool<char>.Shared.Return(arrayForBuffer);

            return foundBlockStart;
        }

        private Span<char> GetPictureFoundBlock(StreamReader iqdbResponseStream, int startIndex)
            => ReadUntilBlockEnd(iqdbResponseStream, "id='show1".AsSpan(), startIndex);

        private Span<char> GetNoMatchBlock(StreamReader iqdbResponseStream, int startIndex)
            => ReadUntilBlockEnd(iqdbResponseStream, "id='show1".AsSpan(), startIndex);

        /// <summary>
        /// Обрабатываем страницу с найденными походими пикчами
        /// </summary>
        /// <param name="iqdbResponseStream">Поток ответа с html страницы</param>
        /// <param name="startIndex">Метка на начало интересующей нас части, считается с НАЧАЛА стрима</param>
        /// <returns>Обработанный ответ с найденными результатами</returns>
        private Span<char> ReadUntilBlockEnd(
            StreamReader iqdbResponseStream,
            ReadOnlySpan<char> blockEnd,
            int startIndex)
        {
            // поставим поток на начало интересующего нас блока
            iqdbResponseStream.BaseStream.Position = startIndex;
            iqdbResponseStream.DiscardBufferedData();

            // подготавливаем буффер и массив для записи блока, который будем обрабатывать дальше
            var bufferFromPool = ArrayPool<char>.Shared.Rent(200);

            char[] blockForProccessing = new char[0];
            Span<char> buffer = bufferFromPool;

            while (!iqdbResponseStream.EndOfStream)
            {
                // считываем и выносим в копию над которой уже безопасно можно проводить манипуляции
                // без опасности повлиять на буффер

                iqdbResponseStream.Read(buffer);
                var currentIterationSpan = buffer;

                // копируем наш стрим в обрабатываемы блок

                var endIndex = currentIterationSpan.IndexOf(blockEnd);
                blockForProccessing = WritePartOfBlock(blockForProccessing, currentIterationSpan, true, 0, endIndex);

                //Если мы нашли конечный блок, то считаем, что блок найден, возвращаем что получилось

                if (endIndex != -1)
                {
                    break;
                }
            }

            ArrayPool<char>.Shared.Return(bufferFromPool);

            if (blockForProccessing.Length == 0)
                throw new Exception($"Can't to found end block for this page, parsing page of type \"Picture found\" failed");

            return blockForProccessing;
        }

        private char[] WritePartOfBlock(char[] blockForProccessing,
                                        Span<char> buffer,
                                        bool isStartFound,
                                        int startBlockIndex,
                                        int endBlockIndex)
        {
            // если мы не нашли начала блока до этого и нет индексов начала и конца в текущем наборе знаков
            // то значит мы ещё не попали в блок и этот кусок можно пропустить

            if (!isStartFound && startBlockIndex == -1 && endBlockIndex == -1)
                return blockForProccessing;

            // если начала так и не нашли, а вот конец нашли - то здесь явно что-то не так

            if (!isStartFound && startBlockIndex == -1 && endBlockIndex != -1)
                throw new Exception("End of block was found, but start of block was skipped");

            // определяем размер копируемого блока
            // если ни начала, ни конца не найдено - копируем целиком
            // иначе ограничиваем либо с начала, либо с конца, либо с обеих сторон сразу

            int sliceStartIndex = startBlockIndex != -1 ? startBlockIndex : 0;
            int sliceEndIndex = endBlockIndex != -1 ? endBlockIndex : buffer.Length;

            // выполняем запись результата в постоянный массив результатов, добавляя новые значения к уже существующим

            blockForProccessing = ConcatinateSpan(blockForProccessing, buffer.Slice(sliceStartIndex, sliceEndIndex - sliceStartIndex));
            return blockForProccessing;
        }

        private char[] ConcatinateSpan(char[] blockForProccessing, Span<char> buffer)
        {
            Span<char> newBlock = ArrayPool<char>.Shared.Rent(blockForProccessing.Length + buffer.Length);
            blockForProccessing.AsSpan().CopyTo(newBlock.Slice(0, blockForProccessing.Length));
            buffer.CopyTo(newBlock.Slice(blockForProccessing.Length, buffer.Length));
            blockForProccessing = newBlock.Slice(0, blockForProccessing.Length + buffer.Length).ToArray();
            ArrayPool<char>.Shared.Return(newBlock.ToArray(), true);
            Span<char> a = blockForProccessing;
            return blockForProccessing;
        }

        private IIqdbImageSearchResult[] ParsePictureFoundBlock(ReadOnlySpan<char> text)
        {
            var results = new List<IIqdbImageSearchResult>();
            results = FindAdditionalMatches(text, "Additional match".AsSpan(), results);
            return results.ToArray();
        }

        private List<IIqdbImageSearchResult> FindAdditionalMatches(ReadOnlySpan<char> span, ReadOnlySpan<char> endBlock, List<IIqdbImageSearchResult> searchResults)
        {
            var endOfCurrentBlock = span.IndexOf(endBlock);
            var currentBlock = endOfCurrentBlock != -1 ? span.Slice(0, endOfCurrentBlock + endBlock.Length).ToString() : span.ToString();
            var match = ParseSearchMatch(currentBlock);
            searchResults.Add(match);

            if (endOfCurrentBlock != -1)
            {
                var trimmedText = span.Slice(endOfCurrentBlock + endBlock.Length);
                searchResults = FindAdditionalMatches(trimmedText, endBlock, searchResults);
            }

            return searchResults;
        }

        private IIqdbImageSearchResult[] ParsePossibleMatches(ReadOnlySpan<char> text)
        {
            var blockSeparator = "Possible match".AsSpan();
            var results = new List<IIqdbImageSearchResult>();

            // в начале блока у нас ненужный текст, пропустим его, оставив только нужное

            var firstBlockSeparatorIndex = text.IndexOf(blockSeparator);

            // если не находим начала, то видимо у нас нет пикчей, возвращаем пустой массив

            if (firstBlockSeparatorIndex == -1)
                return results.ToArray();

            // иначе скипаем бесполезную часть и парсим полезные

            var trimmedText = text.Slice(firstBlockSeparatorIndex + blockSeparator.Length);

            results = FindAdditionalMatches(trimmedText, blockSeparator, results);

            return results.ToArray();
        }

        private IIqdbImageSearchResult ParseSearchMatch(string currentBlock)
        {
            var imageUrlReg = new Regex("(?<=a href=\")[^\"]*");
            var resolutionReg = new Regex("(?<=td>)\\d+×\\d+");
            var eroRatingReg = new Regex("(?<=<td>(\\d{2,8})×(\\d{2,8}\\s)\\[)[^]]*");
            var similarityReg = new Regex("(?<=<td>)\\d+(?=%\\ssimilarity)");

            var imageUriMatch = imageUrlReg.Match(currentBlock);
            var resolutionStringMatch = resolutionReg.Match(currentBlock);
            var eroRatingMatch = eroRatingReg.Match(currentBlock);
            var similarityMatch = similarityReg.Match(currentBlock);

            var isAllParameterFound = imageUriMatch.Success
                                    && resolutionStringMatch.Success
                                    && eroRatingMatch.Success
                                    && similarityMatch.Success;

            if (!isAllParameterFound)
            {
                throw new Exception($"Failed to parse iqdb answer block. uri: {imageUriMatch.Success}; " +
                    $"resolution: {resolutionStringMatch.Success}; " +
                    $"eroRating: {eroRatingMatch.Success};" +
                    $"similarity: {similarityMatch.Success}");
            }

            var imageUri = imageUriMatch.Value;
            var resolution = Resolution.Factory.CreateFromResolutionString(resolutionStringMatch.Value);
            var eroRaing = EroRatingMethods.GetRatingFromString(eroRatingMatch.Value);
            var similarity = int.Parse(similarityMatch.Value);

            var search = IqdbImageSearch.Factory.Create(imageUri, similarity, resolution, eroRaing);

            return search;
        }

        /// <summary>
        /// Рассчитывает максимальное значение из всех представленных вариантов
        /// </summary>
        /// <returns>Максимальное значение</returns>
        private int Max(params int[] args) => args.Max();
    }
}
