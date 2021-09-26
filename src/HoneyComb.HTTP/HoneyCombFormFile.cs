using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace HoneyComb.HTTP
{
    public class HoneyCombFormFile : FormFile
    {
        public HoneyCombFormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName)
            : base(baseStream, baseStreamOffset, length, name, fileName)
        { }

        public HoneyCombFormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName, IHeaderDictionary headers)
            : base(baseStream, baseStreamOffset, length, name, fileName)
        {
            Headers = headers;
        }
    }
}
