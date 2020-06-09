using System;
using System.Collections.Generic;
using System.Text;

namespace AIS.Domain.PictureSearhers
{
    public abstract class SearchResult
    {
        public readonly int SearchId;
        public readonly int SearcherId;
        public readonly bool IsFound;
        protected string _resultString { get; set; }

        public virtual string GetResultInfo()
        {
            if (!string.IsNullOrEmpty(_resultString))
                return _resultString;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Search id: {SearchId}\r\nSearched with {SearchId}\r\n");
            switch (IsFound)
            {
                case true:
                    stringBuilder.AppendLine("Result: File was found");
                    break;
                case false:
                    stringBuilder.AppendLine("Result: Not found");
                    break;
            }

            _resultString = stringBuilder.ToString();
            return _resultString ;
        }
    }
}
