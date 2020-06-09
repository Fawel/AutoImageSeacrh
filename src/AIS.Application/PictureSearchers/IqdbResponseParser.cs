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
        public IqdbSearchResponse ParseResponse(StreamReader iqdbResponseStream, CancellationToken token = default)
        {
            var bufferFromPool = ArrayPool<char>.Shared.Rent(200);
            var bestMatches = "Best match".AsSpan();
            var endBlockString = "id='show1".AsSpan();

            bool startFound = false;

            char[] blockForProccessing = new char[0];

            Span<char> buffer = bufferFromPool;

            while (!iqdbResponseStream.EndOfStream)
            {
                iqdbResponseStream.Read(buffer);
                var currentIterationSpan = buffer;

                var startIndex = currentIterationSpan.IndexOf(bestMatches);
                if (startIndex != -1)
                    startFound = true;

                var endIndex = currentIterationSpan.IndexOf(endBlockString);
                blockForProccessing = WritePartOfBlock(blockForProccessing, currentIterationSpan, startFound, startIndex, endIndex);

                if (endIndex != -1)
                    break;
            }

            var matches = GetImageMatchResult(blockForProccessing);
            ArrayPool<char>.Shared.Return(bufferFromPool);

            var response = IqdbSearchResponse.Factory.Create(matches[0], matches.Length > 1 ? matches[1..] : new IIqdbImageSearchResult[0]);
            return response;
        }

        private char[] WritePartOfBlock(char[] blockForProccessing,
                                        Span<char> buffer,
                                        bool isStartFound,
                                        int startBlockIndex,
                                        int endBlockIndex)
        {
            if (!isStartFound && startBlockIndex == -1 && endBlockIndex == -1)
                return blockForProccessing;

            int sliceStartIndex = startBlockIndex != -1 ? startBlockIndex : 0;
            int sliceEndIndex = endBlockIndex != -1 ? endBlockIndex : buffer.Length;

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

        private IIqdbImageSearchResult[] GetImageMatchResult(ReadOnlySpan<char> text)
        {
            var results = new List<IIqdbImageSearchResult>();
                results = FindAdditionalMatches(text, results);

            static List<IIqdbImageSearchResult> FindAdditionalMatches(ReadOnlySpan<char> span, List<IIqdbImageSearchResult> searchResults)
            {
                var endBlock = "Additional match".AsSpan();
                var endOfCurrentBlock = span.IndexOf(endBlock);
                var currentBlock = endOfCurrentBlock != -1 ? span.Slice(0, endOfCurrentBlock + endBlock.Length).ToString() : span.ToString();
                var match = ParseSearchMatch(currentBlock);
                searchResults.Add(match);

                if (endOfCurrentBlock != -1)
                {
                    var trimmedText = span.Slice(endOfCurrentBlock + endBlock.Length);
                    searchResults = FindAdditionalMatches(trimmedText, searchResults);
                }

                return searchResults;
            }

            return results.ToArray();
        }

        private static IIqdbImageSearchResult ParseSearchMatch(string currentBlock)
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
    }
}
