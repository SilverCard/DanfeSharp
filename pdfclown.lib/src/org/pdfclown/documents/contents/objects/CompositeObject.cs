/*
  Copyright 2007-2011 Stefano Chizzolini. http://www.pdfclown.org

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Composite object. It is made up of multiple content objects.</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public abstract class CompositeObject
    : ContentObject
  {
    #region dynamic
    #region fields
    protected IList<ContentObject> objects;
    #endregion

    #region constructors
    protected CompositeObject(
      )
    {this.objects = new List<ContentObject>();}

    protected CompositeObject(
      ContentObject obj
      ) : this()
    {objects.Add(obj);}

    protected CompositeObject(
      params ContentObject[] objects
      ) : this()
    {
      foreach(ContentObject obj in objects)
      {this.objects.Add(obj);}
    }

    protected CompositeObject(
      IList<ContentObject> objects
      )
    {this.objects = objects;}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the object header.</summary>
    */
    public virtual Operation Header
    {
      get
      {return null;}
      set
      {throw new NotSupportedException();}
    }

    /**
      <summary>Gets the list of inner objects.</summary>
    */
    public IList<ContentObject> Objects
    {
      get
      {return objects;}
    }

    public override void Scan(
      ContentScanner.GraphicsState state
      )
    {
      ContentScanner childLevel = state.Scanner.ChildLevel;

      if(!Render(state))
      {childLevel.MoveEnd();} // Forces the current object to its final graphics state.

      childLevel.State.CopyTo(state); // Copies the current object's final graphics state to the current level's.
    }

    public override string ToString(
      )
    {return "{" + objects.ToString() + "}";}

    public override void WriteTo(
      IOutputStream stream,
      Document context
      )
    {
      foreach(ContentObject obj in objects)
      {obj.WriteTo(stream, context);}
    }
    #endregion

    #region protected
    /**
      <summary>Creates the rendering object corresponding to this container.</summary>
    */
    protected virtual GraphicsPath CreateRenderObject(
      )
    {return null;}

    /**
      <summary>Renders this container.</summary>
      <param name="state">Graphics state.</param>
      <returns>Whether the rendering has been executed.</returns>
     */
    protected bool Render(
      ContentScanner.GraphicsState state
      )
    {
      ContentScanner scanner = state.Scanner;
      Graphics context = scanner.RenderContext;
      if(context == null)
        return false;

      // Render the inner elements!
      scanner.ChildLevel.Render(
        context,
        scanner.CanvasSize,
        CreateRenderObject()
        );
      return true;
    }
    #endregion
    #endregion
    #endregion
  }
}