﻿Imports System.Windows
Imports System.Collections
Imports System.Windows.Controls
Imports DevExpress.Xpf.Grid
Imports DXGridSample.Extensions
Imports System.Collections.Specialized
Imports System

Namespace DXGridSample.Controls
    Public Class MyGridControlBand
        Inherits GridControlBand

        Public Shared ReadOnly ColumnTemplateProperty As DependencyProperty = DependencyProperty.Register("ColumnTemplate", GetType(DataTemplate), GetType(MyGridControlBand), New PropertyMetadata(Nothing))

        Public Property ColumnTemplate() As DataTemplate
            Get
                Return CType(GetValue(ColumnTemplateProperty), DataTemplate)
            End Get
            Set(ByVal value As DataTemplate)
                SetValue(ColumnTemplateProperty, value)
            End Set
        End Property

        Public Shared ReadOnly ColumnTemplateSelectorProperty As DependencyProperty = DependencyProperty.Register("ColumnTemplateSelector", GetType(MyColumnTemplateSelector), GetType(MyGridControlBand), New PropertyMetadata(Nothing))

        Public Property ColumnTemplateSelector() As MyColumnTemplateSelector
            Get
                Return CType(GetValue(ColumnTemplateSelectorProperty), MyColumnTemplateSelector)
            End Get
            Set(ByVal value As MyColumnTemplateSelector)
                SetValue(ColumnTemplateSelectorProperty, value)
            End Set
        End Property


        Public Shared ReadOnly ColumnsSourceProperty As DependencyProperty = DependencyProperty.Register("ColumnsSource", GetType(IList), GetType(MyGridControlBand), New PropertyMetadata(Nothing, AddressOf OnColumnsSourcePropertyChanged))

        Public Property ColumnsSource() As IList
            Get
                Return DirectCast(GetValue(ColumnsSourceProperty), IList)
            End Get
            Set(ByVal value As IList)
                SetValue(ColumnsSourceProperty, value)
            End Set
        End Property

        Private Shared Sub OnColumnsSourcePropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            CType(d, MyGridControlBand).PopulateColumns()

            Dim myDelegate As NotifyCollectionChangedEventHandler = Sub(sender As Object, par As NotifyCollectionChangedEventArgs) CType(d, MyGridControlBand).PopulateColumns()

            If e.OldValue IsNot Nothing AndAlso TypeOf e.OldValue Is INotifyCollectionChanged Then
                RemoveHandler DirectCast(e.NewValue, INotifyCollectionChanged).CollectionChanged, myDelegate
            End If

            If e.NewValue IsNot Nothing AndAlso TypeOf e.NewValue Is INotifyCollectionChanged Then
                AddHandler DirectCast(e.NewValue, INotifyCollectionChanged).CollectionChanged, myDelegate
            End If
        End Sub

        Private Sub PopulateColumns()
            Columns.Clear()
            For Each b In ColumnsSource
                Dim custColumnTemplate As DataTemplate = Nothing
                If ColumnTemplateSelector IsNot Nothing Then
                    custColumnTemplate = ColumnTemplateSelector.SelectTemplate(b, Nothing)
                End If
                If ColumnTemplate IsNot Nothing Then
                    custColumnTemplate = ColumnTemplate
                End If
                If custColumnTemplate IsNot Nothing Then
                    Dim cc As ContentControl = TryCast(custColumnTemplate.LoadContent(), ContentControl)
                    If cc Is Nothing Then
                        Continue For
                    End If
                    Dim column As GridColumn = TryCast(cc.Content, GridColumn)
                    cc.Content = Nothing
                    If column Is Nothing Then
                        Continue For
                    End If
                    column.DataContext = b
                    DependencyObjectHelper.SetDataContext(column, column.DataContext)
                    Columns.Add(column)
                End If
            Next b
        End Sub
    End Class
End Namespace