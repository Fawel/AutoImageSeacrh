using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AIS.Application.PictureSearchers.Models;

namespace AIS.Application.Interfaces.Repositories
{
    public interface ISearchRepository
    {
        /// <summary>
        /// Возвращает id поиска если файл уже искали. Иначе массив пуст
        /// </summary>
        /// <param name="pictureId">Id искомой картинки</param>
        ValueTask<int[]> IsPictureSearched(int pictureId);

        Task GetIqdbSearchDetails(int searchId);

        /// <summary>
        /// Получает информацию о файле для поиска
        /// </summary>
        /// <param name="pictureId">Id искомой картинки</param>
        /// <returns></returns>
        Task<LocalImageInfo> GetPictureFile(int pictureId);
    }
}
