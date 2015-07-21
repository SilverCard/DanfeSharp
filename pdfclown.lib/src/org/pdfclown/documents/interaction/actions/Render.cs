/*
  Copyright 2008-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.interaction.annotations;
using org.pdfclown.documents.multimedia;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>'Control the playing of multimedia content' action [PDF:1.6:8.5.3].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class Render
    : Action
  {
    #region types
    public enum OperationEnum
    {
      /**
        <summary>Play the rendition on the screen, stopping any previous one.</summary>
      */
      Play,
      /**
        <summary>Stop any rendition being played on the screen.</summary>
      */
      Stop,
      /**
        <summary>Pause any rendition being played on the screen.</summary>
      */
      Pause,
      /**
        <summary>Resume any rendition being played on the screen.</summary>
      */
      Resume,
      /**
        <summary>Play the rendition on the screen, resuming any previous one.</summary>
      */
      PlayResume
    }
    #endregion

    #region dynamic
    #region constructors
    /**
      <summary>Creates a new action within the given document context.</summary>
    */
    public Render(
      Screen screen,
      OperationEnum operation,
      Rendition rendition
      ) : base(screen.Document, PdfName.Rendition)
    {
      Operation = operation;
      Screen = screen;
      Rendition = rendition;
    }

    internal Render(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the operation to perform when the action is triggered.</summary>
    */
    public OperationEnum? Operation
    {
      get
      {return OperationEnumExtension.Get((PdfInteger)BaseDataObject[PdfName.OP]);}
      set
      {
        PdfDictionary baseDataObject = BaseDataObject;
        if(value == null && baseDataObject[PdfName.JS] == null)
            throw new ArgumentException("Operation MUST be defined.");

        baseDataObject[PdfName.OP] = (value.HasValue ? value.Value.GetCode() : null);
      }
    }

    /**
      <summary>Gets/Sets the rendition object to render.</summary>
    */
    public Rendition Rendition
    {
      get
      {return Rendition.Wrap(BaseDataObject[PdfName.R]);}
      set
      {
        if(value == null)
        {
          OperationEnum? operation = Operation;
          if(operation.HasValue)
          {
            switch(operation.Value)
            {
              case OperationEnum.Play:
              case OperationEnum.PlayResume:
                throw new ArgumentException("Rendition MUST be defined.");
            }
          }
        }
        BaseDataObject[PdfName.R] = PdfObjectWrapper.GetBaseObject(value);
      }
    }

    /**
      <summary>Gets/Sets the screen where to render the rendition object.</summary>
    */
    public Screen Screen
    {
      get
      {return (Screen)Annotation.Wrap(BaseDataObject[PdfName.AN]);}
      set
      {
        if(value == null)
        {
          OperationEnum? operation = Operation;
          if(operation.HasValue)
          {
            switch(operation.Value)
            {
              case OperationEnum.Play:
              case OperationEnum.PlayResume:
              case OperationEnum.Pause:
              case OperationEnum.Resume:
              case OperationEnum.Stop:
                throw new ArgumentException("Screen MUST be defined.");
            }
          }
        }
        BaseDataObject[PdfName.AN] = PdfObjectWrapper.GetBaseObject(value);
      }
    }

    /**
      <summary>Gets/Sets the JavaScript script to be executed when the action is triggered.</summary>
    */
    public String Script
    {
      get
      {return JavaScript.GetScript(BaseDataObject, PdfName.JS);}
      set
      {
        PdfDictionary baseDataObject = BaseDataObject;
        if(value == null && baseDataObject[PdfName.OP] == null)
            throw new ArgumentException("Script MUST be defined.");

        JavaScript.SetScript(baseDataObject, PdfName.JS, value);
      }
    }
    #endregion
    #endregion
    #endregion
  }

  internal static class OperationEnumExtension
  {
    private static readonly BiDictionary<Render.OperationEnum,PdfInteger> codes;

    static OperationEnumExtension(
      )
    {
      codes = new BiDictionary<Render.OperationEnum,PdfInteger>();
      codes[Render.OperationEnum.Play] = new PdfInteger(0);
      codes[Render.OperationEnum.Stop] = new PdfInteger(1);
      codes[Render.OperationEnum.Pause] = new PdfInteger(2);
      codes[Render.OperationEnum.Resume] = new PdfInteger(3);
      codes[Render.OperationEnum.PlayResume] = new PdfInteger(4);
    }

    public static Render.OperationEnum? Get(
      PdfInteger code
      )
    {
      if(code == null)
        return null;

      Render.OperationEnum? operation = codes.GetKey(code);
      if(!operation.HasValue)
        throw new NotSupportedException("Operation unknown: " + code);

      return operation;
    }

    public static PdfInteger GetCode(
      this Render.OperationEnum operation
      )
    {return codes[operation];}
  }
}