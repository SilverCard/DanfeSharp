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
using org.pdfclown.documents;
using org.pdfclown.documents.contents;
using org.pdfclown.documents.contents.layers;
using colors = org.pdfclown.documents.contents.colorSpaces;
using fonts = org.pdfclown.documents.contents.fonts;
using objects = org.pdfclown.documents.contents.objects;
using org.pdfclown.documents.contents.xObjects;
using actions = org.pdfclown.documents.interaction.actions;
using org.pdfclown.documents.interaction.annotations;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util.math.geom;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.composition
{
  /**
    <summary>
      <para>Content stream primitive composer.</para>
      <para>It provides the basic (primitive) operations described by the PDF specification for
      graphics content composition.</para>
    </summary>
    <remarks>This class leverages the object-oriented content stream modelling infrastructure, which
    encompasses 1st-level content stream objects (operations), 2nd-level content stream objects
    (graphics objects) and full graphics state support.</remarks>
  */
  public sealed class PrimitiveComposer
  {
    #region dynamic
    #region fields
    private ContentScanner scanner;
    #endregion

    #region constructors
    public PrimitiveComposer(
      ContentScanner scanner
      )
    {Scanner = scanner;}

    public PrimitiveComposer(
      IContentContext context
      ) : this(
        new ContentScanner(context.Contents)
        )
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Adds a content object.</summary>
      <returns>The added content object.</returns>
    */
    public objects::ContentObject Add(
      objects::ContentObject obj
      )
    {
      scanner.Insert(obj);
      scanner.MoveNext();

      return obj;
    }

    /**
      <summary>Applies a transformation to the coordinate system from user space to device space
      [PDF:1.6:4.3.3].</summary>
      <remarks>The transformation is applied to the current transformation matrix (CTM) by
      concatenation, i.e. it doesn't replace it.</remarks>
      <param name="a">Item 0,0 of the matrix.</param>
      <param name="b">Item 0,1 of the matrix.</param>
      <param name="c">Item 1,0 of the matrix.</param>
      <param name="d">Item 1,1 of the matrix.</param>
      <param name="e">Item 2,0 of the matrix.</param>
      <param name="f">Item 2,1 of the matrix.</param>
      <seealso cref="SetMatrix(double,double,double,double,double,double)"/>
    */
    public void ApplyMatrix(
      double a,
      double b,
      double c,
      double d,
      double e,
      double f
      )
    {Add(new objects::ModifyCTM(a,b,c,d,e,f));}

    /**
      <summary>Applies the specified state parameters [PDF:1.6:4.3.4].</summary>
      <param name="name">Resource identifier of the state parameters object.</param>
    */
    public void ApplyState(
      PdfName name
      )
    {
      // Doesn't the state exist in the context resources?
      if(!scanner.ContentContext.Resources.ExtGStates.ContainsKey(name))
        throw new ArgumentException("No state resource associated to the given argument.", "name");

      ApplyState_(name);
    }

    /**
      <summary>Applies the specified state parameters [PDF:1.6:4.3.4].</summary>
      <remarks>The <code>value</code> is checked for presence in the current resource dictionary: if
      it isn't available, it's automatically added. If you need to avoid such a behavior, use
      <see cref="ApplyState(PdfName)"/>.</remarks>
      <param name="state">State parameters object.</param>
    */
    public void ApplyState(
      ExtGState state
      )
    {ApplyState_(GetResourceName(state));}

    /**
      <summary>Adds a composite object beginning it.</summary>
      <returns>Added composite object.</returns>
      <seealso cref="End()"/>
    */
    public objects.CompositeObject Begin(
      objects.CompositeObject obj
      )
    {
      // Insert the new object at the current level!
      scanner.Insert(obj);
      // The new object's children level is the new current level!
      scanner = scanner.ChildLevel;

      return obj;
    }

    /**
      <summary>Begins a new layered-content sequence [PDF:1.6:4.10.2].</summary>
      <param name="layer">Layer entity enclosing the layered content.</param>
      <returns>Added layered-content sequence.</returns>
      <seealso cref="End()"/>
    */
    public objects::MarkedContent BeginLayer(
      LayerEntity layer
      )
    {return BeginLayer(GetResourceName(layer.Membership));}

    /**
      <summary>Begins a new layered-content sequence [PDF:1.6:4.10.2].</summary>
      <param name="layerName">Resource identifier of the {@link LayerEntity} enclosing the layered
      content.</param>
      <returns>Added layered-content sequence.</returns>
      <seealso cref="End()"/>
    */
    public objects::MarkedContent BeginLayer(
      PdfName layerName
      )
    {return BeginMarkedContent(PdfName.OC, layerName);}

    /**
      <summary>Begins a new nested graphics state context [PDF:1.6:4.3.1].</summary>
      <returns>Added local graphics state object.</returns>
      <seealso cref="End()"/>
    */
    public objects::LocalGraphicsState BeginLocalState(
      )
    {return (objects::LocalGraphicsState)Begin(new objects::LocalGraphicsState());}

    /**
      <summary>Begins a new marked-content sequence [PDF:1.6:10.5].</summary>
      <param name="tag">Marker indicating the role or significance of the marked content.</param>
      <returns>Added marked-content sequence.</returns>
      <seealso cref="End()"/>
    */
    public objects::MarkedContent BeginMarkedContent(
      PdfName tag
      )
    {return BeginMarkedContent(tag, (PdfName)null);}

    /**
      <summary>Begins a new marked-content sequence [PDF:1.6:10.5].</summary>
      <param name="tag">Marker indicating the role or significance of the marked content.</param>
      <param name="propertyList"><see cref="PropertyList"/> describing the marked content.</param>
      <returns>Added marked-content sequence.</returns>
      <seealso cref="End()"/>
    */
    public objects::MarkedContent BeginMarkedContent(
      PdfName tag,
      PropertyList propertyList
      )
    {return BeginMarkedContent_(tag, GetResourceName(propertyList));}

    /**
      <summary>Begins a new marked-content sequence [PDF:1.6:10.5].</summary>
      <param name="tag">Marker indicating the role or significance of the marked content.</param>
      <param name="propertyListName">Resource identifier of the <see cref="PropertyList"/> describing
      the marked content.</param>
      <returns>Added marked-content sequence.</returns>
      <seealso cref="End()"/>
    */
    public objects::MarkedContent BeginMarkedContent(
      PdfName tag,
      PdfName propertyListName
      )
    {
      // Doesn't the property list exist in the context resources?
      if(propertyListName != null && !scanner.ContentContext.Resources.PropertyLists.ContainsKey(propertyListName))
        throw new ArgumentException("No property list resource associated to the given argument.", "name");

      return BeginMarkedContent_(tag, propertyListName);
    }

    /**
      <summary>Modifies the current clipping path by intersecting it with the current path
      [PDF:1.6:4.4.1].</summary>
      <remarks>It can be validly called only just before painting the current path.</remarks>
    */
    public void Clip(
      )
    {
      Add(objects::ModifyClipPath.NonZero);
      Add(objects::PaintPath.EndPathNoOp);
    }

    /**
      <summary>Closes the current subpath by appending a straight line segment from the current point
      to the starting point of the subpath [PDF:1.6:4.4.1].</summary>
    */
    public void ClosePath(
      )
    {Add(objects::CloseSubpath.Value);}

    /**
      <summary>Draws a circular arc.</summary>
      <param name="location">Arc location.</param>
      <param name="startAngle">Starting angle.</param>
      <param name="endAngle">Ending angle.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawArc(
      RectangleF location,
      double startAngle,
      double endAngle
      )
    {DrawArc(location,startAngle,endAngle,0,1);}

    /**
      <summary>Draws an arc.</summary>
      <param name="location">Arc location.</param>
      <param name="startAngle">Starting angle.</param>
      <param name="endAngle">Ending angle.</param>
      <param name="branchWidth">Distance between the spiral branches. '0' value degrades to a circular
      arc.</param>
      <param name="branchRatio">Linear coefficient applied to the branch width. '1' value degrades to
      a constant branch width.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawArc(
      RectangleF location,
      double startAngle,
      double endAngle,
      double branchWidth,
      double branchRatio
      )
    {DrawArc(location,startAngle,endAngle,branchWidth,branchRatio,true);}

    /**
      <summary>Draws a cubic Bezier curve from the current point [PDF:1.6:4.4.1].</summary>
      <param name="endPoint">Ending point.</param>
      <param name="startControl">Starting control point.</param>
      <param name="endControl">Ending control point.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawCurve(
      PointF endPoint,
      PointF startControl,
      PointF endControl
      )
    {
      double contextHeight = scanner.ContentContext.Box.Height;
      Add(
        new objects::DrawCurve(
          endPoint.X,
          contextHeight - endPoint.Y,
          startControl.X,
          contextHeight - startControl.Y,
          endControl.X,
          contextHeight - endControl.Y
          )
        );
    }

    /**
      <summary>Draws a cubic Bezier curve [PDF:1.6:4.4.1].</summary>
      <param name="startPoint">Starting point.</param>
      <param name="endPoint">Ending point.</param>
      <param name="startControl">Starting control point.</param>
      <param name="endControl">Ending control point.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawCurve(
      PointF startPoint,
      PointF endPoint,
      PointF startControl,
      PointF endControl
      )
    {
      StartPath(startPoint);
      DrawCurve(endPoint,startControl,endControl);
    }

    /**
      <summary>Draws an ellipse.</summary>
      <param name="location">Ellipse location.</param>
      <seealso cref="Fill()"/>
      <seealso cref="FillStroke()"/>
      <seealso cref="Stroke()"/>
    */
    public void DrawEllipse(
      RectangleF location
      )
    {DrawArc(location,0,360);}

    /**
      <summary>Draws a line from the current point [PDF:1.6:4.4.1].</summary>
      <param name="endPoint">Ending point.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawLine(
      PointF endPoint
      )
    {
      Add(
        new objects::DrawLine(
          endPoint.X,
          scanner.ContentContext.Box.Height - endPoint.Y
          )
        );
    }

    /**
      <summary>Draws a line [PDF:1.6:4.4.1].</summary>
      <param name="startPoint">Starting point.</param>
      <param name="endPoint">Ending point.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawLine(
      PointF startPoint,
      PointF endPoint
      )
    {
      StartPath(startPoint);
      DrawLine(endPoint);
    }

    /**
      <summary>Draws a polygon.</summary>
      <remarks>A polygon is the same as a multiple line except that it's a closed path.</remarks>
      <param name="points">Points.</param>
      <seealso cref="Fill()"/>
      <seealso cref="FillStroke()"/>
      <seealso cref="Stroke()"/>
    */
    public void DrawPolygon(
      PointF[] points
      )
    {
      DrawPolyline(points);
      ClosePath();
    }

    /**
      <summary>Draws a multiple line.</summary>
      <param name="points">Points.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawPolyline(
      PointF[] points
      )
    {
      StartPath(points[0]);
      for(
        int index = 1,
          length = points.Length;
        index < length;
        index++
        )
      {DrawLine(points[index]);}
    }

    /**
      <summary>Draws a rectangle [PDF:1.6:4.4.1].</summary>
      <param name="location">Rectangle location.</param>
      <seealso cref="Fill()"/>
      <seealso cref="FillStroke()"/>
      <seealso cref="Stroke()"/>
    */
    public void DrawRectangle(
      RectangleF location
      )
    {DrawRectangle(location,0);}

    /**
      <summary>Draws a rounded rectangle.</summary>
      <param name="location">Rectangle location.</param>
      <param name="radius">Vertex radius, '0' value degrades to squared vertices.</param>
      <seealso cref="Fill()"/>
      <seealso cref="FillStroke()"/>
      <seealso cref="Stroke()"/>
    */
    public void DrawRectangle(
      RectangleF location,
      double radius
      )
    {
      if(radius == 0)
      {
        Add(
          new objects::DrawRectangle(
            location.X,
            scanner.ContentContext.Box.Height - location.Y - location.Height,
            location.Width,
            location.Height
            )
          );
      }
      else
      {
        double endRadians = Math.PI * 2;
        double quadrantRadians = Math.PI / 2;
        double radians = 0;
        while(radians < endRadians)
        {
          double radians2 = radians + quadrantRadians;
          int sin2 = (int)Math.Sin(radians2);
          int cos2 = (int)Math.Cos(radians2);
          double x1 = 0, x2 = 0, y1 = 0, y2 = 0;
          double xArc = 0, yArc = 0;
          if(cos2 == 0)
          {
            if(sin2 == 1)
            {
              x1 = x2 = location.X + location.Width;
              y1 = location.Y + location.Height - radius;
              y2 = location.Y + radius;

              xArc =- radius * 2;
              yArc =- radius;

              StartPath(new PointF((float)x1,(float)y1));
            }
            else
            {
              x1 = x2 = location.X;
              y1 = location.Y + radius;
              y2 = location.Y + location.Height - radius;

              yArc =- radius;
            }
          }
          else if(cos2 == 1)
          {
            x1 = location.X + radius;
            x2 = location.X + location.Width - radius;
            y1 = y2 = location.Y + location.Height;

            xArc =- radius;
            yArc =- radius * 2;
          }
          else if(cos2 == -1)
          {
            x1 = location.X + location.Width - radius;
            x2 = location.X + radius;
            y1 = y2 = location.Y;

            xArc =- radius;
          }
          DrawLine(
            new PointF((float)x2,(float)y2)
            );
          DrawArc(
            new RectangleF((float)(x2+xArc), (float)(y2+yArc), (float)(radius*2), (float)(radius*2)),
            (float)((180 / Math.PI) * radians),
            (float)((180 / Math.PI) * radians2),
            0,
            1,
            false
            );

          radians = radians2;
        }
      }
    }

    /**
      <summary>Draws a spiral.</summary>
      <param name="center">Spiral center.</param>
      <param name="startAngle">Starting angle.</param>
      <param name="endAngle">Ending angle.</param>
      <param name="branchWidth">Distance between the spiral branches.</param>
      <param name="branchRatio">Linear coefficient applied to the branch width.</param>
      <seealso cref="Stroke()"/>
    */
    public void DrawSpiral(
      PointF center,
      double startAngle,
      double endAngle,
      double branchWidth,
      double branchRatio
      )
    {
      DrawArc(
        new RectangleF(center.X, center.Y, 0.0001f, 0.0001f),
        startAngle,
        endAngle,
        branchWidth,
        branchRatio
        );
    }

    /**
      <summary>Ends the current (innermostly-nested) composite object.</summary>
      <seealso cref="Begin(CompositeObject)"/>
    */
    public void End(
      )
    {
      scanner = scanner.ParentLevel;
      scanner.MoveNext();
    }

    /**
      <summary>Fills the path using the current color [PDF:1.6:4.4.2].</summary>
      <seealso cref="SetFillColor(Color)"/>
    */
    public void Fill(
      )
    {Add(objects::PaintPath.Fill);}

    /**
      <summary>Fills and then strokes the path using the current colors [PDF:1.6:4.4.2].</summary>
      <seealso cref="SetFillColor(Color)"/>
      <seealso cref="SetStrokeColor(Color)"/>
    */
    public void FillStroke(
      )
    {Add(objects::PaintPath.FillStroke);}

    /**
      <summary>Serializes the contents into the content stream.</summary>
    */
    public void Flush(
      )
    {scanner.Contents.Flush();}

    /**
      <summary>Gets/Sets the content stream scanner.</summary>
    */
    public ContentScanner Scanner
    {
      get
      {return scanner;}
      set
      {scanner = value;}
    }

    /**
      <summary>Gets the current graphics state [PDF:1.6:4.3].</summary>
    */
    public ContentScanner.GraphicsState State
    {
      get
      {return scanner.State;}
    }

    /**
      <summary>Applies a rotation to the coordinate system from user space to device space
      [PDF:1.6:4.2.2].</summary>
      <param name="angle">Rotational counterclockwise angle.</param>
      <seealso cref="ApplyMatrix(double,double,double,double,double,double)"/>
    */
    public void Rotate(
      double angle
      )
    {
      double rad = angle * Math.PI / 180;
      double cos = Math.Cos(rad);
      double sin = Math.Sin(rad);
      ApplyMatrix(cos, sin, -sin, cos, 0, 0);
    }

    /**
      <summary>Applies a rotation to the coordinate system from user space to device space
      [PDF:1.6:4.2.2].</summary>
      <param name="angle">Rotational counterclockwise angle.</param>
      <param name="origin">Rotational pivot point; it becomes the new coordinates origin.</param>
      <seealso cref="ApplyMatrix(double,double,double,double,double,double)"/>
    */
    public void Rotate(
      double angle,
      PointF origin
      )
    {
      // Center to the new origin!
      Translate(
        origin.X,
        scanner.ContentContext.Box.Height - origin.Y
        );
      // Rotate on the new origin!
      Rotate(angle);
      // Restore the standard vertical coordinates system!
      Translate(
        0,
        -scanner.ContentContext.Box.Height
        );
    }

    /**
      <summary>Applies a scaling to the coordinate system from user space to device space
      [PDF:1.6:4.2.2].</summary>
      <param name="ratioX">Horizontal scaling ratio.</param>
      <param name="ratioY">Vertical scaling ratio.</param>
      <seealso cref="ApplyMatrix(double,double,double,double,double,double)"/>
    */
    public void Scale(
      double ratioX,
      double ratioY
      )
    {ApplyMatrix(ratioX, 0, 0, ratioY, 0, 0);}

    /**
      <summary>Sets the character spacing parameter [PDF:1.6:5.2.1].</summary>
    */
    public void SetCharSpace(
      double value
      )
    {Add(new objects::SetCharSpace(value));}

    /**
      <summary>Sets the nonstroking color value [PDF:1.6:4.5.7].</summary>
      <seealso cref="SetStrokeColor(Color)"/>
    */
    public void SetFillColor(
      colors::Color value
      )
    {
      if(!scanner.State.FillColorSpace.Equals(value.ColorSpace))
      {
        // Set filling color space!
        Add(new objects::SetFillColorSpace(GetResourceName(value.ColorSpace)));
      }

      Add(new objects::SetFillColor(value));
    }

    /**
      <summary>Sets the font [PDF:1.6:5.2].</summary>
      <param name="name">Resource identifier of the font.</param>
      <param name="size">Scaling factor (points).</param>
    */
    public void SetFont(
      PdfName name,
      double size
      )
    {
      // Doesn't the font exist in the context resources?
      if(!scanner.ContentContext.Resources.Fonts.ContainsKey(name))
        throw new ArgumentException("No font resource associated to the given argument.","name");

      SetFont_(name,size);
    }

    /**
      <summary>Sets the font [PDF:1.6:5.2].</summary>
      <remarks>The <paramref cref="value"/> is checked for presence in the current resource
      dictionary: if it isn't available, it's automatically added. If you need to avoid such a
      behavior, use <see cref="SetFont(PdfName,double)"/>.</remarks>
      <param name="value">Font.</param>
      <param name="size">Scaling factor (points).</param>
    */
    public void SetFont(
      fonts::Font value,
      double size
      )
    {SetFont_(GetResourceName(value),size);}

    /**
      <summary>Sets the text horizontal scaling [PDF:1.6:5.2.3].</summary>
    */
    public void SetTextScale(
      double value
      )
    {Add(new objects::SetTextScale(value));}

    /**
      <summary>Sets the text leading [PDF:1.6:5.2.4].</summary>
    */
    public void SetTextLead(
      double value
      )
    {Add(new objects::SetTextLead(value));}

    /**
      <summary>Sets the line cap style [PDF:1.6:4.3.2].</summary>
    */
    public void SetLineCap(
      LineCapEnum value
      )
    {Add(new objects::SetLineCap(value));}

    /**
      <summary>Sets the line dash pattern [PDF:1.6:4.3.2].</summary>
    */
    public void SetLineDash(
      LineDash value
      )
    {Add(new objects::SetLineDash(value));}

    /**
      <summary>Sets the line join style [PDF:1.6:4.3.2].</summary>
    */
    public void SetLineJoin(
      LineJoinEnum value
      )
    {Add(new objects::SetLineJoin(value));}

    /**
      <summary>Sets the line width [PDF:1.6:4.3.2].</summary>
    */
    public void SetLineWidth(
      double value
      )
    {Add(new objects::SetLineWidth(value));}

    /**
      <summary>Sets the transformation of the coordinate system from user space to device space
      [PDF:1.6:4.3.3].</summary>
      <param name="a">Item 0,0 of the matrix.</param>
      <param name="b">Item 0,1 of the matrix.</param>
      <param name="c">Item 1,0 of the matrix.</param>
      <param name="d">Item 1,1 of the matrix.</param>
      <param name="e">Item 2,0 of the matrix.</param>
      <param name="f">Item 2,1 of the matrix.</param>
      <seealso cref="ApplyMatrix(double,double,double,double,double,double)"/>
    */
    public void SetMatrix(
      double a,
      double b,
      double c,
      double d,
      double e,
      double f
      )
    {
      // Reset the CTM!
      Add(objects::ModifyCTM.GetResetCTM(scanner.State));
      // Apply the transformation!
      Add(new objects::ModifyCTM(a,b,c,d,e,f));
    }

    /**
      <summary>Sets the miter limit [PDF:1.6:4.3.2].</summary>
    */
    public void SetMiterLimit(
      double value
      )
    {Add(new objects::SetMiterLimit(value));}

    /**
      <summary>Sets the stroking color value [PDF:1.6:4.5.7].</summary>
      <seealso cref="SetFillColor(Color)"/>
    */
    public void SetStrokeColor(
      colors::Color value
      )
    {
      if(!scanner.State.StrokeColorSpace.Equals(value.ColorSpace))
      {
        // Set stroking color space!
        Add(new objects::SetStrokeColorSpace(GetResourceName(value.ColorSpace)));
      }

      Add(new objects::SetStrokeColor(value));
    }

    /**
      <summary>Sets the text rendering mode [PDF:1.6:5.2.5].</summary>
    */
    public void SetTextRenderMode(
      TextRenderModeEnum value
      )
    {Add(new objects::SetTextRenderMode(value));}

    /**
      <summary>Sets the text rise [PDF:1.6:5.2.6].</summary>
    */
    public void SetTextRise(
      double value
      )
    {Add(new objects::SetTextRise(value));}

    /**
      <summary>Sets the word spacing [PDF:1.6:5.2.2].</summary>
    */
    public void SetWordSpace(
      double value
      )
    {Add(new objects::SetWordSpace(value));}

    /**
      <summary>Shows the specified text on the page at the current location [PDF:1.6:5.3.2].</summary>
      <param name="value">Text to show.</param>
      <returns>Bounding box vertices in default user space units.</returns>
    */
    public Quad ShowText(
      string value
      )
    {
      return ShowText(
        value,
        new PointF(0,0)
        );
    }

    /**
      <summary>Shows the link associated to the specified text on the page at the current location.
      </summary>
      <param name="value">Text to show.</param>
      <param name="action">Action to apply when the link is activated.</param>
      <returns>Link.</returns>
    */
    public Link ShowText(
      string value,
      actions::Action action
      )
    {
      return ShowText(
        value,
        new PointF(0,0),
        action
        );
    }

    /**
      <summary>Shows the specified text on the page at the specified location [PDF:1.6:5.3.2].
      </summary>
      <param name="value">Text to show.</param>
      <param name="location">Position at which showing the text.</param>
      <returns>Bounding box vertices in default user space units.</returns>
    */
    public Quad ShowText(
      string value,
      PointF location
      )
    {
      return ShowText(
        value,
        location,
        XAlignmentEnum.Left,
        YAlignmentEnum.Top,
        0
        );
    }

    /**
      <summary>Shows the link associated to the specified text on the page at the specified location.
      </summary>
      <param name="value">Text to show.</param>
      <param name="location">Position at which showing the text.</param>
      <param name="action">Action to apply when the link is activated.</param>
      <returns>Link.</returns>
    */
    public Link ShowText(
      string value,
      PointF location,
      actions::Action action
      )
    {
      return ShowText(
        value,
        location,
        XAlignmentEnum.Left,
        YAlignmentEnum.Top,
        0,
        action
        );
    }

    /**
      <summary>Shows the specified text on the page at the specified location [PDF:1.6:5.3.2].
      </summary>
      <param name="value">Text to show.</param>
      <param name="location">Anchor position at which showing the text.</param>
      <param name="xAlignment">Horizontal alignment.</param>
      <param name="yAlignment">Vertical alignment.</param>
      <param name="rotation">Rotational counterclockwise angle.</param>
      <returns>Bounding box vertices in default user space units.</returns>
    */
    public Quad ShowText(
      string value,
      PointF location,
      XAlignmentEnum xAlignment,
      YAlignmentEnum yAlignment,
      double rotation
      )
    {
      ContentScanner.GraphicsState state = scanner.State;
      fonts::Font font = state.Font;
      double fontSize = state.FontSize;
      double x = location.X;
      double y = location.Y;
      double width = font.GetKernedWidth(value,fontSize);
      double height = font.GetLineHeight(fontSize);
      double descent = font.GetDescent(fontSize);
      Quad frame;
      if(xAlignment == XAlignmentEnum.Left
        && yAlignment == YAlignmentEnum.Top)
      {
        BeginText();
        try
        {
          if(rotation == 0)
          {
            TranslateText(
              x,
              scanner.ContentContext.Box.Height - y - font.GetAscent(fontSize)
              );
          }
          else
          {
            double rad = rotation * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            SetTextMatrix(
              cos,
              sin,
              -sin,
              cos,
              x,
              scanner.ContentContext.Box.Height - y - font.GetAscent(fontSize)
              );
          }

          state = scanner.State;
          frame = new Quad(
            state.TextToDeviceSpace(new PointF(0, (float)descent), true),
            state.TextToDeviceSpace(new PointF((float)width, (float)descent), true),
            state.TextToDeviceSpace(new PointF((float)width, (float)(height + descent)), true),
            state.TextToDeviceSpace(new PointF(0, (float)(height + descent)), true)
            );

          // Add the text!
          Add(new objects::ShowSimpleText(font.Encode(value)));
        }
        finally
        {End();} // Ends the text object.
      }
      else
      {
        BeginLocalState();
        try
        {
          // Coordinates transformation.
          double cos, sin;
          if(rotation == 0)
          {
            cos = 1;
            sin = 0;
          }
          else
          {
            double rad = rotation * Math.PI / 180.0;
            cos = Math.Cos(rad);
            sin = Math.Sin(rad);
          }
          // Apply the transformation!
          ApplyMatrix(
            cos,
            sin,
            -sin,
            cos,
            x,
            scanner.ContentContext.Box.Height - y
            );

          // Begin the text object!
          BeginText();
          try
          {
            // Text coordinates adjustment.
            switch(xAlignment)
            {
              case XAlignmentEnum.Left:
                x = 0;
                break;
              case XAlignmentEnum.Right:
                x = -width;
                break;
              case XAlignmentEnum.Center:
              case XAlignmentEnum.Justify:
                x = -width / 2;
                break;
            }
            switch(yAlignment)
            {
              case YAlignmentEnum.Top:
                y = -font.GetAscent(fontSize);
                break;
              case YAlignmentEnum.Bottom:
                y = height - font.GetAscent(fontSize);
                break;
              case YAlignmentEnum.Middle:
                y = height / 2 - font.GetAscent(fontSize);
                break;
            }
            // Apply the text coordinates adjustment!
            TranslateText(x,y);

            state = scanner.State;
            frame = new Quad(
              state.TextToDeviceSpace(new PointF(0, (float)descent), true),
              state.TextToDeviceSpace(new PointF((float)width, (float)descent), true),
              state.TextToDeviceSpace(new PointF((float)width, (float)(height + descent)), true),
              state.TextToDeviceSpace(new PointF(0, (float)(height + descent)), true)
              );

            // Add the text!
            Add(new objects::ShowSimpleText(font.Encode(value)));
          }
          finally
          {End();} // Ends the text object.
        }
        finally
        {End();} // Ends the local state.
      }
      return frame;
    }

    /**
      <summary>Shows the link associated to the specified text on the page at the specified location.
      </summary>
      <param name="value">Text to show.</param>
      <param name="location">Anchor position at which showing the text.</param>
      <param name="xAlignment">Horizontal alignment.</param>
      <param name="yAlignment">Vertical alignment.</param>
      <param name="rotation">Rotational counterclockwise angle.</param>
      <param name="action">Action to apply when the link is activated.</param>
      <returns>Link.</returns>
    */
    public Link ShowText(
      string value,
      PointF location,
      XAlignmentEnum xAlignment,
      YAlignmentEnum yAlignment,
      double rotation,
      actions::Action action
      )
    {
      IContentContext contentContext = scanner.ContentContext;
      if(!(contentContext is Page))
        throw new Exception("Links can be shown only on page contexts.");

      RectangleF linkBox = ShowText(
        value,
        location,
        xAlignment,
        yAlignment,
        rotation
        ).GetBounds();

      return new Link(
        (Page)contentContext,
        linkBox,
        null,
        action
        );
    }

    /**
      <summary>Shows the specified external object [PDF:1.6:4.7].</summary>
      <param name="name">Resource identifier of the external object.</param>
    */
    public void ShowXObject(
      PdfName name
      )
    {Add(new objects::PaintXObject(name));}

    /**
      <summary>Shows the specified external object [PDF:1.6:4.7].</summary>
      <remarks>The <paramref cref="value"/> is checked for presence in the current resource
      dictionary: if it isn't available, it's automatically added. If you need to avoid such a
      behavior, use <see cref="ShowXObject(PdfName)"/>.</remarks>
      <param name="value">External object.</param>
    */
    public void ShowXObject(
      XObject value
      )
    {ShowXObject(GetResourceName(value));}

    /**
      <summary>Shows the specified external object at the specified position [PDF:1.6:4.7].</summary>
      <param name="name">Resource identifier of the external object.</param>
      <param name="location">Position at which showing the external object.</param>
    */
    public void ShowXObject(
      PdfName name,
      PointF location
      )
    {
      ShowXObject(
        name,
        location,
        null
        );
    }

    /**
      <summary>Shows the specified external object at the specified position [PDF:1.6:4.7].</summary>
      <remarks>The <paramref cref="value"/> is checked for presence in the current resource
      dictionary: if it isn't available, it's automatically added. If you need to avoid such a
      behavior, use <see cref="ShowXObject(PdfName,PointF)"/>.</remarks>
      <param name="value">External object.</param>
      <param name="location">Position at which showing the external object.</param>
    */
    public void ShowXObject(
      XObject value,
      PointF location
      )
    {
      ShowXObject(
        GetResourceName(value),
        location
        );
    }

    /**
      <summary>Shows the specified external object at the specified position [PDF:1.6:4.7].</summary>
      <param name="name">Resource identifier of the external object.</param>
      <param name="location">Position at which showing the external object.</param>
      <param name="size">Size of the external object.</param>
    */
    public void ShowXObject(
      PdfName name,
      PointF location,
      SizeF? size
      )
    {
      ShowXObject(
        name,
        location,
        size,
        XAlignmentEnum.Left,
        YAlignmentEnum.Top,
        0
        );
    }

    /**
      <summary>Shows the specified external object at the specified position [PDF:1.6:4.7].</summary>
      <remarks>The <paramref cref="value"/> is checked for presence in the current resource
      dictionary: if it isn't available, it's automatically added. If you need to avoid such a
      behavior, use <see cref="ShowXObject(PdfName,PointF,SizeF)"/>.</remarks>
      <param name="value">External object.</param>
      <param name="location">Position at which showing the external object.</param>
      <param name="size">Size of the external object.</param>
    */
    public void ShowXObject(
      XObject value,
      PointF location,
      SizeF? size
      )
    {
      ShowXObject(
        GetResourceName(value),
        location,
        size
        );
    }

    /**
      <summary>Shows the specified external object at the specified position [PDF:1.6:4.7].</summary>
      <param name="name">Resource identifier of the external object.</param>
      <param name="location">Position at which showing the external object.</param>
      <param name="size">Size of the external object.</param>
      <param name="xAlignment">Horizontal alignment.</param>
      <param name="yAlignment">Vertical alignment.</param>
      <param name="rotation">Rotational counterclockwise angle.</param>
    */
    public void ShowXObject(
      PdfName name,
      PointF location,
      SizeF? size,
      XAlignmentEnum xAlignment,
      YAlignmentEnum yAlignment,
      double rotation
      )
    {
      XObject xObject = scanner.ContentContext.Resources.XObjects[name];
      SizeF xObjectSize = xObject.Size;

      if(!size.HasValue)
      {size = xObjectSize;}

      // Scaling.
      Matrix matrix = xObject.Matrix;
      double scaleX, scaleY;
      scaleX = size.Value.Width / (xObjectSize.Width * matrix.Elements[0]);
      scaleY = size.Value.Height / (xObjectSize.Height * matrix.Elements[3]);

      // Alignment.
      float locationOffsetX, locationOffsetY;
      switch(xAlignment)
      {
        case XAlignmentEnum.Left:
          locationOffsetX = 0;
          break;
        case XAlignmentEnum.Right:
          locationOffsetX = size.Value.Width;
          break;
        case XAlignmentEnum.Center:
        case XAlignmentEnum.Justify:
        default:
          locationOffsetX = size.Value.Width / 2;
          break;
      }
      switch(yAlignment)
      {
        case YAlignmentEnum.Top:
          locationOffsetY = size.Value.Height;
          break;
        case YAlignmentEnum.Bottom:
          locationOffsetY = 0;
          break;
        case YAlignmentEnum.Middle:
        default:
          locationOffsetY = size.Value.Height / 2;
          break;
      }

      BeginLocalState();
      try
      {
        Translate(
          location.X,
          scanner.ContentContext.Box.Height - location.Y
          );
        if(rotation != 0)
        {Rotate(rotation);}
        ApplyMatrix(
          scaleX, 0, 0,
          scaleY,
          -locationOffsetX,
          -locationOffsetY
          );
        ShowXObject(name);
      }
      finally
      {End();} // Ends the local state.
    }

    /**
      <summary>Shows the specified external object at the specified position [PDF:1.6:4.7].</summary>
      <remarks>The <paramref cref="value"/> is checked for presence in the current resource
      dictionary: if it isn't available, it's automatically added. If you need to avoid such a
      behavior, use <see cref="ShowXObject(PdfName,PointF,SizeF,XAlignmentEnum,YAlignmentEnum,double)"/>.
      </remarks>
      <param name="value">External object.</param>
      <param name="location">Position at which showing the external object.</param>
      <param name="size">Size of the external object.</param>
      <param name="xAlignment">Horizontal alignment.</param>
      <param name="yAlignment">Vertical alignment.</param>
      <param name="rotation">Rotational counterclockwise angle.</param>
    */
    public void ShowXObject(
      XObject value,
      PointF location,
      SizeF? size,
      XAlignmentEnum xAlignment,
      YAlignmentEnum yAlignment,
      double rotation
      )
    {
      ShowXObject(
        GetResourceName(value),
        location,
        size,
        xAlignment,
        yAlignment,
        rotation
        );
    }

    /**
      <summary>Begins a subpath [PDF:1.6:4.4.1].</summary>
      <param name="startPoint">Starting point.</param>
    */
    public void StartPath(
      PointF startPoint
      )
    {
      Add(
        new objects::BeginSubpath(
          startPoint.X,
          scanner.ContentContext.Box.Height - startPoint.Y
          )
        );
    }

    /**
      <summary>Strokes the path using the current color [PDF:1.6:4.4.2].</summary>
      <seealso cref="SetStrokeColor(Color)"/>
    */
    public void Stroke(
      )
    {Add(objects::PaintPath.Stroke);}

    /**
      <summary>Applies a translation to the coordinate system from user space to device space
      [PDF:1.6:4.2.2].</summary>
      <param name="distanceX">Horizontal distance.</param>
      <param name="distanceY">Vertical distance.</param>
      <seealso cref="ApplyMatrix(double,double,double,double,double,double)"/>
    */
    public void Translate(
      double distanceX,
      double distanceY
      )
    {ApplyMatrix(1, 0, 0, 1, distanceX, distanceY);}
    #endregion

    #region private
    private void ApplyState_(
      PdfName name
      )
    {Add(new objects::ApplyExtGState(name));}

    private objects::MarkedContent BeginMarkedContent_(
      PdfName tag,
      PdfName propertyListName
      )
    {
      return (objects::MarkedContent)Begin(
        new objects::MarkedContent(
          new objects::BeginMarkedContent(tag, propertyListName)
          )
        );
    }

    /**
      <summary>Begins a text object [PDF:1.6:5.3].</summary>
      <seealso cref="End()"/>
    */
    private objects::Text BeginText(
      )
    {return (objects::Text)Begin(new objects::Text());}

    //TODO: drawArc MUST seamlessly manage already-begun paths.
    private void DrawArc(
      RectangleF location,
      double startAngle,
      double endAngle,
      double branchWidth,
      double branchRatio,
      bool beginPath
      )
    {
      /*
        NOTE: Strictly speaking, arc drawing is NOT a PDF primitive;
        it leverages the cubic bezier curve operator (thanks to
        G. Adam Stanislav, whose article was greatly inspirational:
        see http://www.whizkidtech.redprince.net/bezier/circle/).
      */

      if(startAngle > endAngle)
      {
        double swap = startAngle;
        startAngle = endAngle;
        endAngle = swap;
      }

      double radiusX = location.Width / 2;
      double radiusY = location.Height / 2;

      PointF center = new PointF(
        (float)(location.X + radiusX),
        (float)(location.Y + radiusY)
        );

      double radians1 = (Math.PI / 180) * startAngle;
      PointF point1 = new PointF(
        (float)(center.X + Math.Cos(radians1) * radiusX),
        (float)(center.Y - Math.Sin(radians1) * radiusY)
        );

      if(beginPath)
      {StartPath(point1);}

      double endRadians = (Math.PI / 180) * endAngle;
      double quadrantRadians = Math.PI / 2;
      double radians2 = Math.Min(
        radians1 + quadrantRadians - radians1 % quadrantRadians,
        endRadians
        );
      double kappa = 0.5522847498;
      while(true)
      {
        double segmentX = radiusX * kappa;
        double segmentY = radiusY * kappa;

        // Endpoint 2.
        PointF point2 = new PointF(
          (float)(center.X + Math.Cos(radians2) * radiusX),
          (float)(center.Y - Math.Sin(radians2) * radiusY)
          );

        // Control point 1.
        double tangentialRadians1 = Math.Atan(
          -(Math.Pow(radiusY,2) * (point1.X-center.X))
            / (Math.Pow(radiusX,2) * (point1.Y-center.Y))
          );
        double segment1 = (
          segmentY * (1 - Math.Abs(Math.Sin(radians1)))
            + segmentX * (1 - Math.Abs(Math.Cos(radians1)))
          ) * (radians2-radians1) / quadrantRadians; // TODO: control segment calculation is still not so accurate as it should -- verify how to improve it!!!
        PointF control1 = new PointF(
          (float)(point1.X + Math.Abs(Math.Cos(tangentialRadians1) * segment1) * Math.Sign(-Math.Sin(radians1))),
          (float)(point1.Y + Math.Abs(Math.Sin(tangentialRadians1) * segment1) * Math.Sign(-Math.Cos(radians1)))
          );

        // Control point 2.
        double tangentialRadians2 = Math.Atan(
          -(Math.Pow(radiusY,2) * (point2.X-center.X))
            / (Math.Pow(radiusX,2) * (point2.Y-center.Y))
          );
        double segment2 = (
          segmentY * (1 - Math.Abs(Math.Sin(radians2)))
            + segmentX * (1 - Math.Abs(Math.Cos(radians2)))
          ) * (radians2-radians1) / quadrantRadians; // TODO: control segment calculation is still not so accurate as it should -- verify how to improve it!!!
        PointF control2 = new PointF(
          (float)(point2.X + Math.Abs(Math.Cos(tangentialRadians2) * segment2) * Math.Sign(Math.Sin(radians2))),
          (float)(point2.Y + Math.Abs(Math.Sin(tangentialRadians2) * segment2) * Math.Sign(Math.Cos(radians2)))
          );

        // Draw the current quadrant curve!
        DrawCurve(
          point2,
          control1,
          control2
          );

        // Last arc quadrant?
        if(radians2 == endRadians)
          break;

        // Preparing the next quadrant iteration...
        point1 = point2;
        radians1 = radians2;
        radians2 += quadrantRadians;
        if(radians2 > endRadians)
        {radians2 = endRadians;}

        double quadrantRatio = (radians2 - radians1) / quadrantRadians;
        radiusX += branchWidth * quadrantRatio;
        radiusY += branchWidth * quadrantRatio;

        branchWidth *= branchRatio;
      }
    }

    private PdfName GetResourceName<T>(
      T value
      ) where T : PdfObjectWrapper
    {
      if(value is colors::DeviceGrayColorSpace)
        return PdfName.DeviceGray;
      else if(value is colors::DeviceRGBColorSpace)
        return PdfName.DeviceRGB;
      else if(value is colors::DeviceCMYKColorSpace)
        return PdfName.DeviceCMYK;
      else
      {
        // Ensuring that the resource exists within the context resources...
        PdfDictionary resourceItemsObject = ((PdfObjectWrapper<PdfDictionary>)scanner.ContentContext.Resources.Get(value.GetType())).BaseDataObject;
        // Get the key associated to the resource!
        PdfName name = resourceItemsObject.GetKey(value.BaseObject);
        // No key found?
        if(name == null)
        {
          // Insert the resource within the collection!
          int resourceIndex = resourceItemsObject.Count;
          do
          {name = new PdfName((++resourceIndex).ToString());}
          while(resourceItemsObject.ContainsKey(name));
          resourceItemsObject[name] = value.BaseObject;
        }
        return name;
      }
    }

    /**
      <summary>Applies a rotation to the coordinate system from text space to user space
      [PDF:1.6:4.2.2].</summary>
      <param name="angle">Rotational counterclockwise angle.</param>
    */
    private void RotateText(
      double angle
      )
    {
      double rad = angle * Math.PI / 180;
      double cos = Math.Cos(rad);
      double sin = Math.Sin(rad);

      SetTextMatrix(cos, sin, -sin, cos, 0, 0);
    }

    /**
      <summary>Applies a scaling to the coordinate system from text space to user space
      [PDF:1.6:4.2.2].</summary>
      <param name="ratioX">Horizontal scaling ratio.</param>
      <param name="ratioY">Vertical scaling ratio.</param>
    */
    private void ScaleText(
      double ratioX,
      double ratioY
      )
    {SetTextMatrix(ratioX, 0, 0, ratioY, 0, 0);}

    private void SetFont_(
      PdfName name,
      double size
      )
    {Add(new objects::SetFont(name,size));}

    /**
      <summary>Sets the transformation of the coordinate system from text space to user space
      [PDF:1.6:5.3.1].</summary>
      <remarks>The transformation replaces the current text matrix.</remarks>
      <param name="a">Item 0,0 of the matrix.</param>
      <param name="b">Item 0,1 of the matrix.</param>
      <param name="c">Item 1,0 of the matrix.</param>
      <param name="d">Item 1,1 of the matrix.</param>
      <param name="e">Item 2,0 of the matrix.</param>
      <param name="f">Item 2,1 of the matrix.</param>
    */
    private void SetTextMatrix(
      double a,
      double b,
      double c,
      double d,
      double e,
      double f
      )
    {Add(new objects::SetTextMatrix(a,b,c,d,e,f));}

    /**
      <summary>Applies a translation to the coordinate system from text space to user space
      [PDF:1.6:4.2.2].</summary>
      <param name="distanceX">Horizontal distance.</param>
      <param name="distanceY">Vertical distance.</param>
    */
    private void TranslateText(
      double distanceX,
      double distanceY
      )
    {SetTextMatrix(1, 0, 0, 1, distanceX, distanceY);}

    /**
      <summary>Applies a translation to the coordinate system from text space to user space,
      relative to the start of the current line [PDF:1.6:5.3.1].</summary>
      <param name="offsetX">Horizontal offset.</param>
      <param name="offsetY">Vertical offset.</param>
    */
    private void TranslateTextRelative(
      double offsetX,
      double offsetY
      )
    {
      Add(
        new objects::TranslateTextRelative(
          offsetX,
          -offsetY
          )
        );
    }

    /**
      <summary>Applies a translation to the coordinate system from text space to user space,
      moving to the start of the next line [PDF:1.6:5.3.1].</summary>
    */
    private void TranslateTextToNextLine(
      )
    {Add(objects::TranslateTextToNextLine.Value);}
    #endregion
    #endregion
    #endregion
  }
}