using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Compression;

namespace Umbraco.Courier.RepositoryProviders.WebserviceProvider.Compression
{
    public class Compression
    {

        public static byte[] Compress(byte[] data)
        {
            return data;
            /*
            using (var compressedStream = new MemoryStream())
                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    zipStream.Write(data, 0, data.Length);
                    zipStream.Close();
                    return compressedStream.ToArray();
                }*/
        }

        
        public static byte[] Decompress(byte[] data)
        {
            return data;
            /*
            using(var compressedStream = new MemoryStream(data))
                using(var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                using (var resultStream = new MemoryStream())
                {
                    zipStream.Write(data, 0, data.Length);
                    zipStream.Close();
                    return compressedStream.ToArray();
                }*/
        }
    }
}