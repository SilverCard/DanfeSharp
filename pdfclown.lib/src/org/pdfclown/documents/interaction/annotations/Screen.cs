/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents;
using org.pdfclown.documents.files;
using org.pdfclown.documents.interaction.actions;
using org.pdfclown.documents.interaction.forms;
using org.pdfclown.documents.multimedia;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using drawing = System.Drawing;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Screen annotation [PDF:1.6:8.4.5].</summary>
    <remarks>It specifies a region of a page upon which media clips may be played.</remarks>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class Screen
    : Annotation
  {
    #region static
    #region fields
    private const string PlayerPlaceholder = "%player%";
    /**
      <summary>Script for preview and rendering control.</summary>
      <remarks>NOTE: PlayerPlaceholder MUST be replaced with the actual player instance symbol.
      </remarks>
    */
    private const string RenderScript = "if(" + PlayerPlaceholder + "==undefined){"
      + "var doc = this;"
      + "var settings={autoPlay:false,visible:false,volume:100,startAt:0};"
      + "var events=new app.media.Events({"
        + "afterFocus:function(event){try{if(event.target.isPlaying){event.target.pause();}else{event.target.play();}doc.getField('" + PlayerPlaceholder + "').setFocus();}catch(e){}},"
        + "afterReady:function(event){try{event.target.seek(event.target.settings.startAt);event.target.visible=true;}catch(e){}}"
        + "});"
      + "var " + PlayerPlaceholder + "=app.media.openPlayer({settings:settings,events:events});"
      + "}";
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Screen(
      Page page,
      drawing::RectangleF box,
      String text,
      String mediaPath,
      String mimeType
      ) : this(
        page, box, text,
        new MediaRendition(
          new MediaClipData(
            FileSpecification.Get(
              EmbeddedFile.Get(page.Document, mediaPath),
              System.IO.Path.GetFileName(mediaPath)
              ),
            mimeType
            )
          )
        )
    {}

    public Screen(
      Page page,
      drawing::RectangleF box,
      String text,
      Rendition rendition
      ) : base(page, PdfName.Screen, box, text)
    {
      Render render = new Render(this, Render.OperationEnum.PlayResume, rendition);
      {
        // Adding preview and play/pause control...
        /*
          NOTE: Mouse-related actions don't work when the player is active; therefore, in order to let
          the user control the rendering of the media clip (play/pause) just by mouse-clicking on the
          player, we can only rely on the player's focus event. Furthermore, as the player's focus can
          only be altered setting it on another widget, we have to define an ancillary field on the
          same page (so convoluted!).
        */
        string playerReference = "__player" + ((PdfReference)render.BaseObject).ObjectNumber;
        Document.Form.Fields.Add(new TextField(playerReference, new Widget(page, new drawing::RectangleF(box.X, box.Y, 0, 0)), "")); // Ancillary field.
        render.Script = RenderScript.Replace(PlayerPlaceholder, playerReference);
      }
      Actions.OnPageOpen = render;

      if(rendition is MediaRendition)
      {
        PdfObjectWrapper data = ((MediaRendition)rendition).Clip.Data;
        if(data is FileSpecification)
        {
          // Adding fallback annotation...
          /*
            NOTE: In case of viewers which don't support video rendering, this annotation gently
            degrades to a file attachment that can be opened on the same location of the corresponding
            screen annotation.
          */
          FileAttachment attachment = new FileAttachment(page, box, text, (FileSpecification)data);
          BaseDataObject[PdfName.T] = PdfString.Get(((FileSpecification)data).Path);
          // Force empty appearance to ensure no default icon is drawn on the canvas!
          attachment.BaseDataObject[PdfName.AP] = new PdfDictionary(new PdfName[]{PdfName.D, PdfName.R, PdfName.N}, new PdfDirectObject[]{new PdfDictionary(), new PdfDictionary(), new PdfDictionary()});
        }
      }
    }

    internal Screen(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion
    #endregion
  }
}