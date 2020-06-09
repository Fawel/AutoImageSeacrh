using System;
using System.Collections.Generic;
using System.Text;

namespace AIS.Domain.PictureSearhers
{
    public class PictureSearch
    {
        public int Id { get; private set; }
        public List<int> PictureIds { get; private set; }
        public readonly List<Searcher> Searchers;

        public void AddSearcher(Searcher searcher)
        {

        }

        public void RemoveSearcher(Searcher searcher)
        {

        }

        public void AddPictureToSearch(int pictureId)
        {
            PictureIds.Add(pictureId);
        }

        public void RemovePictureFromSearch(int pictureId)
        {
            PictureIds.Remove(pictureId);
        }
    }
}
