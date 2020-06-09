using AIS.Domain.ImageFiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.Interfaces.Repositories
{
    public interface IImageFileRepository
    {
        Task<int> SaveImageFile(ImageFile imageFile, CancellationToken token = default);
        Task<ImageFile> GetByMd5(Md5Info md5Info, CancellationToken token = default);
        Task<ImageFile[]> GetAllImageFiles(CancellationToken token = default);
    }
}
