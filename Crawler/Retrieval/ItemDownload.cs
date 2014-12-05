using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Retrieval
{
    /// <summary>
    /// This class holds all data for asynchroneous file retrieval
    /// </summary>
    internal class ItemDownload<T>
    {
        internal Uri Uri;
        internal T UserContext;
        internal int Retry = 0;
        internal int Priority = 0;
    }
}
