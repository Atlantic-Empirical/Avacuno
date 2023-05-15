Imports System.Math
Imports System.Drawing

Module MiscGraphics

    Private Const GRAB_HANDLE_WID As Integer = 25
    Private Const GRAB_HANDLE_RADIUS As Integer = GRAB_HANDLE_WID

    ' Clicks this close to an object hit the object.
    Public Const CLOSE_DIST As Integer = 25

    ' Calculate the distance from a point to a segment.
    Public Function DistToSegment(ByVal px As Double, ByVal py As Double, ByVal X1 As Double, ByVal Y1 As Double, ByVal X2 As Double, ByVal Y2 As Double) As Double
        Dim dx As Double
        Dim dy As Double
        Dim t As Double

        dx = X2 - X1
        dy = Y2 - Y1
        If dx = 0 And dy = 0 Then
            ' It's a point not a line segment.
            dx = px - X1
            dy = py - Y1
            Return Sqrt(dx * dx + dy * dy)
        End If

        t = (dx * (px - X1) + dy * (py - Y1)) / (dx * dx + dy * dy)
        If t < 0 Then
            dx = px - X1
            dy = py - Y1
        ElseIf t > 1 Then
            dx = px - X2
            dy = py - Y2
        Else
            X2 = X1 + t * dx
            Y2 = Y1 + t * dy
            dx = px - X2
            dy = py - Y2
        End If
        Return Sqrt(dx * dx + dy * dy)
    End Function

    ' Return true if the point (X, Y) is close 
    ' to the segment (X1, Y1)-(X2, Y2).
    Public Function PointCloseToSegment(ByVal X As Integer, ByVal Y As Integer, ByVal X1 As Integer, ByVal Y1 As Integer, ByVal X2 As Integer, ByVal Y2 As Integer) As Boolean
        Return DistToSegment(X, Y, X1, Y1, X2, Y2) <= MiscGraphics.CLOSE_DIST
    End Function

    ' Draw a grab handle centered at (Cx, Cy).
    Public Sub DrawGrabHandle(ByVal gr As Graphics, ByVal Cx As Single, ByVal Cy As Integer)
        gr.FillRectangle(Brushes.White, _
            Cx - GRAB_HANDLE_WID \ 2, _
            Cy - GRAB_HANDLE_WID \ 2, _
            GRAB_HANDLE_WID, GRAB_HANDLE_WID)
        gr.DrawRectangle(Pens.Black, _
            Cx - GRAB_HANDLE_WID \ 2, _
            Cy - GRAB_HANDLE_WID \ 2, _
            GRAB_HANDLE_WID, GRAB_HANDLE_WID)
    End Sub

    ' Return True if (X, Y) is near (X1, Y1).
    Public Function PointsAreClose(ByVal X As Integer, ByVal Y As Integer, ByVal X1 As Integer, ByVal Y1 As Integer) As Boolean
        Return (Abs(X - X1) <= GRAB_HANDLE_RADIUS) AndAlso (Abs(Y - Y1) <= GRAB_HANDLE_RADIUS)
    End Function

End Module

Public Class cAnchorPoint
    Public Offset As Byte
    Public Ellipse As DrawableEllipse

    Public Property X() As Short
        Get
            Return Me.Ellipse.BoundingBox.X
        End Get
        Set(ByVal value As Short)
            Dim bb As Rectangle = Me.Ellipse.BoundingBox
            bb.X = value
            Me.Ellipse.BoundingBox = bb
        End Set
    End Property

    Public Property Y() As Short
        Get
            Return Me.Ellipse.BoundingBox.Y
        End Get
        Set(ByVal value As Short)
            Dim bb As Rectangle = Me.Ellipse.BoundingBox
            bb.Y = value
            Me.Ellipse.BoundingBox = bb
        End Set
    End Property

    Public ReadOnly Property X2() As Short
        Get
            Return Me.Ellipse.BoundingBox.X + Me.Ellipse.BoundingBox.Width
        End Get
    End Property

    Public ReadOnly Property Y2() As Short
        Get
            Return Me.Ellipse.BoundingBox.Y + Me.Ellipse.BoundingBox.Height
        End Get
    End Property

    Public Property TrueCenter_X() As Short
        Get
            Return Me.X + (Offset / 2)
        End Get
        Set(ByVal value As Short)
            Me.X = value - (Offset / 2)
        End Set
    End Property

    Public Property TrueCenter_Y() As Short
        Get
            Return Me.Y + (Offset / 2)
        End Get
        Set(ByVal value As Short)
            Me.Y = value - (Offset / 2)
        End Set
    End Property

    Public ReadOnly Property Width() As Short
        Get
            Return Abs(X - X2)
        End Get
    End Property

    Public ReadOnly Property Height() As Short
        Get
            Return Abs(Y - Y2)
        End Get
    End Property

    Public Sub New(ByVal nOffset As Byte)
        Me.Offset = nOffset
        Me.Ellipse = New DrawableEllipse(Pens.Blue, Brushes.Blue, "AnchorPoint")
        Me.Ellipse.BoundingBox = New Rectangle((1920 / 2) - (Offset / 2), (1080 / 2) - (Offset / 2), Offset, Offset)
    End Sub

End Class

Public Class cRadial

    Public Line As DrawableLine

    Public Property X2() As Short
        Get
            Return Line.X2
        End Get
        Set(ByVal value As Short)
            Line.X2 = value
        End Set
    End Property

    Public Property Y2() As Short
        Get
            Return Line.Y2
        End Get
        Set(ByVal value As Short)
            Line.Y2 = value
        End Set
    End Property

    Public Sub New()
    End Sub

End Class

Public Enum HitType
    None
    Adjust
    UndoRedoMove
    Body
    GrabHandleUL
    GrabHandleUR
    GrabHandleLL
    GrabHandleLR

    GrabHandleMin = GrabHandleUL
    GrabHandleMax = GrabHandleLR
End Enum

Public MustInherit Class Drawable
    Public Selected As Boolean
    Public Pen As Pen
    Public Brush As Brush
    Public Display As Boolean = True
    Public Name As String

    Public MustOverride Function GetBoundingBox() As Rectangle

    ' Draw the object.
    Public MustOverride Sub Draw(ByVal gr As Graphics, ByRef AP As cAnchorPoint)

    ' Return a value indicating whether the object is over this point.
    Public MustOverride Function IsAt(ByVal X As Integer, ByVal Y As Integer) As HitType

    ' Return a new copy of the object.
    Public MustOverride Function Clone() As Drawable

    ' Resize the object.
    Public MustOverride Sub Adjust(ByVal hit_type As HitType, ByVal dx As Integer, ByVal dy As Integer, Optional ByVal nX1 As Short = 0, Optional ByVal nY1 As Short = 0, Optional ByVal nX2 As Short = 0, Optional ByVal nY2 As Short = 0)

End Class

Public Class DrawableLine
    Inherits Drawable

    Private AnchorPoint As cAnchorPoint
    Public TopHalfRadial As Boolean = False

    Public ReadOnly Property X1() As Short
        Get
            Return Me.AnchorPoint.TrueCenter_X
        End Get
    End Property

    Public ReadOnly Property Y1() As Short
        Get
            Return Me.AnchorPoint.TrueCenter_Y
        End Get
    End Property

    Private _X2 As Short
    Public Property X2() As Short
        Get
            Return _X2
        End Get
        Set(ByVal value As Short)
            If Y2 < 30 Or Y2 > 1050 Then
                'line goes to bottom or top
                If value < 0 Then
                    _X2 = 0
                    Debug.WriteLine("Value less than 0")
                ElseIf value > 1920 Then
                    _X2 = 1920
                    Debug.WriteLine("Value greater than 1920")
                Else
                    _X2 = value
                End If
            Else
                'line goes to side
                If value > 960 Then
                    _X2 = 1920
                Else
                    _X2 = 0
                End If
            End If
            'Debug.WriteLine("_X2 = " & _X2)
        End Set
    End Property

    Private _Y2 As Short
    Public Property Y2() As Short
        Get
            Return _Y2
        End Get
        Set(ByVal value As Short)
            If Me.TopHalfRadial Then
                If value > Me.AnchorPoint.TrueCenter_Y Then
                    Exit Property
                End If
            Else
                If value < Me.AnchorPoint.TrueCenter_Y Then
                    Exit Property
                End If
            End If
            If value < 0 Then
                _Y2 = 0
                Debug.WriteLine("Value less than 0")
            ElseIf value > 1080 Then
                _Y2 = 1080
                Debug.WriteLine("Value greater than 1920")
            Else
                _Y2 = value
            End If
            'Debug.WriteLine("_Y2 = " & _Y2)
        End Set
    End Property

    Public Overrides Function GetBoundingBox() As Rectangle
        Return New Rectangle(X1, Y1, Abs(X2 - X1), Abs(Y2 - Y1))
    End Function

    ' Initialize the pen and brush.
    Public Sub New(ByVal new_pen As Pen, ByRef AP As cAnchorPoint, ByVal new_x1 As Integer, ByVal new_y1 As Integer, ByVal new_x2 As Integer, ByVal new_y2 As Integer, ByVal TopHalf As Boolean)
        Pen = new_pen
        Me.AnchorPoint = AP
        X2 = new_x2
        Y2 = new_y2
        Me.TopHalfRadial = TopHalf
    End Sub

    Public Overrides Sub Draw(ByVal gr As System.Drawing.Graphics, ByRef AP As cAnchorPoint)
        AnchorPoint = AP
        If Selected Then
            ' Draw in orange.
            gr.DrawLine(New Pen(Color.Orange, 10), X1, Y1, X2, Y2)

            ' Draw grab handles.
            DrawGrabHandle(gr, X1, Y1)
            DrawGrabHandle(gr, X2, Y2)
        Else
            'Debug.WriteLine("DrawLine_Draw: X1 = " & X1 & " Y1 = " & Y1 & " X2 = " & X2 & " Y2 = " & Y2)
            gr.DrawLine(Pen, X1, Y1, X2, Y2)
        End If
    End Sub

    ' Return True if the point is inside the rectangle.
    Public Overrides Function IsAt(ByVal X As Integer, ByVal Y As Integer) As HitType
        ' See if the point is at a grab handle.
        'If PointsAreClose(X, Y, X1, Y1) Then Return HitType.GrabHandleUL
        If PointsAreClose(X, Y, X2, Y2) Then Return HitType.GrabHandleLR

        ' See if the point is near the line.
        'If PointCloseToSegment(X, Y, X1, Y1, X2, Y2) Then Return HitType.Body

        Return HitType.None
    End Function

    ' Return a new copy of the object.
    Public Overrides Function Clone() As Drawable
        Return New DrawableLine(Pen, Me.AnchorPoint, X1, Y1, X2, Y2, Me.TopHalfRadial)
    End Function

    ' Adjust the object for a drag handle move.
    Public Overrides Sub Adjust(ByVal hit_type As HitType, ByVal dx As Integer, ByVal dy As Integer, Optional ByVal nX1 As Short = 0, Optional ByVal nY1 As Short = 0, Optional ByVal nX2 As Short = 0, Optional ByVal nY2 As Short = 0)
        Y2 += dy
        X2 += dx
        'Debug.WriteLine("DrawLine_Adjust: X2 = " & X2 & " Y2 = " & Y2)
    End Sub

End Class

Public Class DrawableEllipse
    Inherits Drawable

    Private _BoundingBox As Rectangle
    Public Property BoundingBox() As Rectangle
        Get
            Return _BoundingBox
        End Get
        Set(ByVal value As Rectangle)
            _BoundingBox = value
        End Set
    End Property

    ' Initialize the pen and brush.
    Public Sub New(ByVal new_pen As Pen, ByVal new_brush As Brush, ByVal nName As String)
        Name = nName
        Pen = new_pen
        Brush = new_brush
    End Sub

    Public Overrides Sub Draw(ByVal gr As System.Drawing.Graphics, ByRef AP As cAnchorPoint)
        If Selected Then
            ' Draw in orange.
            gr.FillEllipse(Brushes.Orange, BoundingBox)
            gr.DrawEllipse(New Pen(Color.Orange, Pen.Width), BoundingBox)

            ' Draw grab handles.
            DrawGrabHandle(gr, BoundingBox.X, BoundingBox.Y)
            DrawGrabHandle(gr, BoundingBox.X + BoundingBox.Width, BoundingBox.Y)
            DrawGrabHandle(gr, BoundingBox.X, BoundingBox.Y + BoundingBox.Height)
            DrawGrabHandle(gr, BoundingBox.X + BoundingBox.Width, BoundingBox.Y + BoundingBox.Height)
        Else
            gr.FillEllipse(Brush, BoundingBox)
            gr.DrawEllipse(Pen, BoundingBox)
        End If
    End Sub

    ' Return True if the point is inside the rectangle.
    Public Overrides Function IsAt(ByVal X As Integer, ByVal Y As Integer) As HitType
        ' See if the point is at a grab handle.
        If PointsAreClose(X, Y, BoundingBox.X, BoundingBox.Y) Then Return HitType.GrabHandleUL
        If PointsAreClose(X, Y, BoundingBox.X, BoundingBox.Y + BoundingBox.Height) Then Return HitType.GrabHandleLL
        If PointsAreClose(X, Y, BoundingBox.X + BoundingBox.Width, BoundingBox.Y) Then Return HitType.GrabHandleUR
        If PointsAreClose(X, Y, BoundingBox.X + BoundingBox.Width, BoundingBox.Y + BoundingBox.Height) Then Return HitType.GrabHandleLR

        ' See if the point is inside the bounding box.
        If BoundingBox.Contains(X, Y) Then Return HitType.Body

        Return HitType.None
    End Function

    Public Overrides Function GetBoundingBox() As Rectangle
        Return Me.BoundingBox
    End Function

    ' Return a new copy of the object.
    Public Overrides Function Clone() As Drawable
        Return New DrawableEllipse(Pen, Brush, Name)
    End Function

    ' Adjust the object for a drag handle move.
    Public Overrides Sub Adjust(ByVal hit_type As HitType, ByVal dx As Integer, ByVal dy As Integer, Optional ByVal nX1 As Short = 0, Optional ByVal nY1 As Short = 0, Optional ByVal nX2 As Short = 0, Optional ByVal nY2 As Short = 0)
        Dim new_x1 As Integer = BoundingBox.X
        Dim new_y1 As Integer = BoundingBox.Y
        Dim new_x2 As Integer = BoundingBox.X + BoundingBox.Width
        Dim new_y2 As Integer = BoundingBox.Y + BoundingBox.Height

        Select Case hit_type
            'Case HitType.GrabHandleUL
            '    new_x1 += dx
            '    new_y1 += dy
            'Case HitType.GrabHandleUR
            '    new_x2 += dx
            '    new_y1 += dy
            'Case HitType.GrabHandleLL
            '    new_x1 += dx
            '    new_y2 += dy
            'Case HitType.GrabHandleLR
            '    new_x2 += dx
            '    new_y2 += dy
            Case HitType.Body
                new_x1 += dx
                new_y1 += dy
                new_x2 += dx
                new_y2 += dy
                'Case HitType.Adjust
                '    new_x1 = nX1
                '    new_y1 = nY1
                '    new_x2 = nX2
                '    new_y2 = nY2
        End Select

        BoundingBox = New Rectangle( _
            Min(new_x1, new_x2), Min(new_y1, new_y2), _
            Abs(new_x1 - new_x2), Abs(new_y1 - new_y2))

    End Sub

End Class

Public Class colDrawables
    Inherits CollectionBase

    Public Function Add(ByVal newButton As Drawable) As Integer
        Return MyBase.List.Add(newButton)
    End Function

    Default Public ReadOnly Property Item(ByVal index As Integer) As Drawable
        Get
            Return MyBase.List.Item(index)
        End Get
    End Property

    Public Sub Remove(ByVal Item As Drawable)
        MyBase.List.Remove(Item)
    End Sub
End Class
