using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIS.Application.Interfaces.Repositories
{
    public interface ISearchRepository
    {
        Task<string> GetPictureFile(int pictureId);
    }
}
