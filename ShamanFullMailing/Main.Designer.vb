<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.barManagerAll = New DevExpress.XtraBars.BarManager(Me.components)
        Me.Bar3 = New DevExpress.XtraBars.Bar()
        Me.stLog = New DevExpress.XtraBars.BarStaticItem()
        Me.barDockControlTop = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlBottom = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlLeft = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlRight = New DevExpress.XtraBars.BarDockControl()
        Me.btnCerrar = New DevExpress.XtraBars.BarButtonItem()
        Me.tmrInit = New System.Windows.Forms.Timer(Me.components)
        Me.grdLog = New DevExpress.XtraGrid.GridControl()
        Me.vewLog = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.ID = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.Comprobante = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.Destino = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.Observaciones = New DevExpress.XtraGrid.Columns.GridColumn()
        CType(Me.barManagerAll, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.grdLog, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.vewLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'barManagerAll
        '
        Me.barManagerAll.Bars.AddRange(New DevExpress.XtraBars.Bar() {Me.Bar3})
        Me.barManagerAll.DockControls.Add(Me.barDockControlTop)
        Me.barManagerAll.DockControls.Add(Me.barDockControlBottom)
        Me.barManagerAll.DockControls.Add(Me.barDockControlLeft)
        Me.barManagerAll.DockControls.Add(Me.barDockControlRight)
        Me.barManagerAll.Form = Me
        Me.barManagerAll.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.btnCerrar, Me.stLog})
        Me.barManagerAll.MaxItemId = 2
        Me.barManagerAll.StatusBar = Me.Bar3
        '
        'Bar3
        '
        Me.Bar3.BarName = "Status bar"
        Me.Bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom
        Me.Bar3.DockCol = 0
        Me.Bar3.DockRow = 0
        Me.Bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom
        Me.Bar3.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.stLog)})
        Me.Bar3.OptionsBar.AllowQuickCustomization = False
        Me.Bar3.OptionsBar.DrawDragBorder = False
        Me.Bar3.OptionsBar.UseWholeRow = True
        Me.Bar3.Text = "Status bar"
        '
        'stLog
        '
        Me.stLog.Id = 1
        Me.stLog.Name = "stLog"
        '
        'barDockControlTop
        '
        Me.barDockControlTop.CausesValidation = False
        Me.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.barDockControlTop.Location = New System.Drawing.Point(0, 0)
        Me.barDockControlTop.Manager = Me.barManagerAll
        Me.barDockControlTop.Size = New System.Drawing.Size(893, 0)
        '
        'barDockControlBottom
        '
        Me.barDockControlBottom.CausesValidation = False
        Me.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.barDockControlBottom.Location = New System.Drawing.Point(0, 256)
        Me.barDockControlBottom.Manager = Me.barManagerAll
        Me.barDockControlBottom.Size = New System.Drawing.Size(893, 22)
        '
        'barDockControlLeft
        '
        Me.barDockControlLeft.CausesValidation = False
        Me.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left
        Me.barDockControlLeft.Location = New System.Drawing.Point(0, 0)
        Me.barDockControlLeft.Manager = Me.barManagerAll
        Me.barDockControlLeft.Size = New System.Drawing.Size(0, 256)
        '
        'barDockControlRight
        '
        Me.barDockControlRight.CausesValidation = False
        Me.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right
        Me.barDockControlRight.Location = New System.Drawing.Point(893, 0)
        Me.barDockControlRight.Manager = Me.barManagerAll
        Me.barDockControlRight.Size = New System.Drawing.Size(0, 256)
        '
        'btnCerrar
        '
        Me.btnCerrar.Caption = "Cerrar"
        Me.btnCerrar.Id = 0
        Me.btnCerrar.ImageOptions.Image = CType(resources.GetObject("btnCerrar.ImageOptions.Image"), System.Drawing.Image)
        Me.btnCerrar.Name = "btnCerrar"
        '
        'tmrInit
        '
        Me.tmrInit.Interval = 8000
        '
        'grdLog
        '
        Me.grdLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grdLog.Location = New System.Drawing.Point(0, 0)
        Me.grdLog.MainView = Me.vewLog
        Me.grdLog.MenuManager = Me.barManagerAll
        Me.grdLog.Name = "grdLog"
        Me.grdLog.Size = New System.Drawing.Size(893, 256)
        Me.grdLog.TabIndex = 15
        Me.grdLog.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.vewLog})
        '
        'vewLog
        '
        Me.vewLog.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.ID, Me.Comprobante, Me.Destino, Me.Observaciones})
        Me.vewLog.GridControl = Me.grdLog
        Me.vewLog.Name = "vewLog"
        Me.vewLog.OptionsView.ShowGroupPanel = False
        '
        'ID
        '
        Me.ID.AppearanceHeader.Options.UseTextOptions = True
        Me.ID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.ID.Caption = "ID"
        Me.ID.FieldName = "ID"
        Me.ID.Name = "ID"
        Me.ID.OptionsColumn.AllowEdit = False
        Me.ID.OptionsColumn.ReadOnly = True
        '
        'Comprobante
        '
        Me.Comprobante.AppearanceHeader.Options.UseTextOptions = True
        Me.Comprobante.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.Comprobante.Caption = "Comprobante"
        Me.Comprobante.FieldName = "Comprobante"
        Me.Comprobante.Name = "Comprobante"
        Me.Comprobante.OptionsColumn.AllowEdit = False
        Me.Comprobante.OptionsColumn.ReadOnly = True
        Me.Comprobante.Visible = True
        Me.Comprobante.VisibleIndex = 0
        Me.Comprobante.Width = 149
        '
        'Destino
        '
        Me.Destino.AppearanceHeader.Options.UseTextOptions = True
        Me.Destino.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.Destino.Caption = "Destino"
        Me.Destino.FieldName = "Destino"
        Me.Destino.Name = "Destino"
        Me.Destino.OptionsColumn.AllowEdit = False
        Me.Destino.OptionsColumn.ReadOnly = True
        Me.Destino.Visible = True
        Me.Destino.VisibleIndex = 1
        Me.Destino.Width = 266
        '
        'Observaciones
        '
        Me.Observaciones.AppearanceHeader.Options.UseTextOptions = True
        Me.Observaciones.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.Observaciones.Caption = "Observaciones"
        Me.Observaciones.FieldName = "Observaciones"
        Me.Observaciones.Name = "Observaciones"
        Me.Observaciones.OptionsColumn.AllowEdit = False
        Me.Observaciones.OptionsColumn.ReadOnly = True
        Me.Observaciones.Visible = True
        Me.Observaciones.VisibleIndex = 2
        Me.Observaciones.Width = 460
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(893, 278)
        Me.Controls.Add(Me.grdLog)
        Me.Controls.Add(Me.barDockControlLeft)
        Me.Controls.Add(Me.barDockControlRight)
        Me.Controls.Add(Me.barDockControlBottom)
        Me.Controls.Add(Me.barDockControlTop)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Main"
        Me.Text = "Shaman Mailing"
        CType(Me.barManagerAll, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.grdLog, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.vewLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents barManagerAll As DevExpress.XtraBars.BarManager
    Friend WithEvents btnCerrar As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents Bar3 As DevExpress.XtraBars.Bar
    Friend WithEvents barDockControlTop As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlBottom As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlLeft As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlRight As DevExpress.XtraBars.BarDockControl
    Friend WithEvents tmrInit As System.Windows.Forms.Timer
    Friend WithEvents stLog As DevExpress.XtraBars.BarStaticItem
    Friend WithEvents grdLog As DevExpress.XtraGrid.GridControl
    Friend WithEvents vewLog As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents ID As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Comprobante As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Destino As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Observaciones As DevExpress.XtraGrid.Columns.GridColumn
End Class
