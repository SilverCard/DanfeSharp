using DanfeSharp.Graphics;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp
{
    internal class SimplePdfPageTester : IDisposable
    {
        public File File { get; set; }
        public Document Document { get; set; }
        public PrimitiveComposer PrimitiveComposer { get; set; }
        public Gfx Gfx { get; set; }
        public Page Page { get; set; }

        public SimplePdfPageTester()
        {            
            File = new File();
            Document = File.Document;

            Page = new Page(Document);
            Document.Pages.Add(Page);

            PrimitiveComposer = new PrimitiveComposer(Page);
            Gfx = new Gfx(PrimitiveComposer);
        }

        public void Save(String path)
        {
            File.Save(path, SerializationModeEnum.Standard);
        }

        public Estilo CriarEstilo()
        {
            return new Estilo(new StandardType1Font(Document, StandardType1Font.FamilyEnum.Times, false, false),
                      new StandardType1Font(Document, StandardType1Font.FamilyEnum.Times, true, false),
                      new StandardType1Font(Document, StandardType1Font.FamilyEnum.Times, false, true));
        }

        public void Save()
        {
            Save(new StackTrace().GetFrame(1).GetMethod().Name + ".pdf");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (File != null) File.Dispose();
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SimplePdfPageTester() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
