﻿using SharpFileDB.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Services
{
    public class DiskService : IDisposable
    {
        private FileStream fileStream;

        private BinaryReader binaryReader;
        private BinaryWriter binaryWriter;

        public DiskService(string fullname)
        {
            var stream = new FileStream(fullname, FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite, Pages.PageAddress.PAGE_SIZE);
            this.fileStream = stream;
            this.binaryReader = new BinaryReader(stream);
            this.binaryWriter = new BinaryWriter(stream);
        }

        public T ReadPage<T>(ulong pageID) where T : BasePage
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public void WritePage(BasePage page)
        {
            var stream = this.fileStream;
            long posStart, posEnd;
            checked
            {
                ulong start = (page.pageHeaderInfo.pageID * PageAddress.PAGE_SIZE);
                ulong end = start + PageAddress.PAGE_SIZE;
                if ((ulong)(long.MaxValue) < end)
                { throw new Exception(string.Format("{0} is too far away as a FileStream.Position", end)); }
                posStart = (long)start;
                posEnd = (long)end;
            }

            // position cursor
            if (stream.Position != posStart)
            {
                stream.Seek(posStart, SeekOrigin.Begin);
            }

            // write page header
            page.WriteHeader(this.binaryWriter);

            // write content 
            page.WriteContent(this.binaryWriter);

            // write with zero non-used page
            if (stream.Position < posEnd)
            {
                this.binaryWriter.Write(new byte[posEnd - stream.Position]);
            }

            // if page is dirty, clean up
            page.IsDirty = false;
        }


        #region IDisposable Members

        /// <summary>
        /// Internal variable which checks if Dispose has already been called
        /// </summary>
        private Boolean disposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(Boolean disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                //TODO: Managed cleanup code here, while managed refs still valid
            }
            //TODO: Unmanaged cleanup code here
            this.binaryReader.Close();
            this.binaryWriter.Close();
            this.fileStream.Close();

            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        #endregion
				
    }
}