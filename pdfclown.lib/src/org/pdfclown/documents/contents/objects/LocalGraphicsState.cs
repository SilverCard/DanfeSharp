/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

  Contributors:
    * Stefano Chizzolini (original code developer, http://www.stefanochizzolini.it)

  This file should be part of the source code distribution of "PDF Clown library" (the
  Program): see the accompanying README files for more info.

  This Program is free software; you can redistribute it and/or modify it under the terms
  of the GNU Lesser General Public License as published by the Free Software Foundation;
  either version 3 of the License, or (at your option) any later version.

  This Program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
  either expressed or implied; without even the implied warranty of MERCHANTABILITY or
  FITNESS FOR A PARTICULAR PURPOSE. See the License for more details.

  You should have received a copy of the GNU Lesser General Public License along with this
  Program (see README files); if not, go to the GNU website (http://www.gnu.org/licenses/).

  Redistribution and use, with or without modification, are permitted provided that such
  redistributions retain the above copyright notice, license and disclaimer, along with
  this list of conditions.
*/

using org.pdfclown.bytes;
using org.pdfclown.objects;
using org.pdfclown.tokens;

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Local graphics state [PDF:1.6:4.3.1].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class LocalGraphicsState
    : ContainerObject
  {
    #region static
    #region fields
    public static readonly string BeginOperatorKeyword = SaveGraphicsState.OperatorKeyword;
    public static readonly string EndOperatorKeyword = RestoreGraphicsState.OperatorKeyword;

    private static readonly byte[] BeginChunk = Encoding.Pdf.Encode(BeginOperatorKeyword + Symbol.LineFeed);
    private static readonly byte[] EndChunk = Encoding.Pdf.Encode(EndOperatorKeyword + Symbol.LineFeed);
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public LocalGraphicsState(
      )
    {}

    public LocalGraphicsState(
      IList<ContentObject> objects
      ) : base(objects)
    {}
    #endregion

    #region interface
    #region public
    public override void Scan(
      ContentScanner.GraphicsState state
      )
    {
      Graphics context = state.Scanner.RenderContext;
      if(context != null)
      {
        /*
          NOTE: Local graphics state is purposely isolated from surrounding graphics state,
          so no inner operation can alter its subsequent scanning.
        */
        // Save outer graphics state!
        GraphicsState contextState = context.Save();

        Render(state);

        // Restore outer graphics state!
        context.Restore(contextState);
      }
    }

    public override void WriteTo(
      IOutputStream stream,
      Document context
      )
    {
      stream.Write(BeginChunk);
      base.WriteTo(stream, context);
      stream.Write(EndChunk);
    }
    #endregion
    #endregion
    #endregion
  }
}