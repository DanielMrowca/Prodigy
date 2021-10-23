using System.IO;
using Microsoft.AspNetCore.Http;

namespace Prodigy.HTTP
{
    public class ProdigyFormFile : FormFile
    {
        public ProdigyFormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName)
            : base(baseStream, baseStreamOffset, length, name, fileName)
        { }

        public ProdigyFormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName, IHeaderDictionary headers)
            : base(baseStream, baseStreamOffset, length, name, fileName)
        {
            Headers = headers;
        }
    }
}
