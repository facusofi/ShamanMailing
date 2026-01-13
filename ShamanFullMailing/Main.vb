Imports System.Configuration
Imports ShamanClases
Imports ShamanClases_CSharp

Public Class Main

    Private dtLog As New DataTable
    Private vEmailsTop As Integer = 1
    Private IsResend As Boolean = False

    Private Sub Main_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Try
            ShamanSession.Cerrar(ShamanSession.PID)
        Catch ex As Exception
            HandleError(Me.Name, "Main_FormClosed", ex)
        End Try
    End Sub

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try

            Dim cacheServer As String = ConfigurationManager.AppSettings("CacheServer").ToString()
            Dim cachePort As Long = Val(ConfigurationManager.AppSettings("CachePort").ToString())
            Dim cacheNameSpace As String = ConfigurationManager.AppSettings("CacheNameSpace").ToString()
            Dim cacheApp As String = ConfigurationManager.AppSettings("CacheApp").ToString()
            Dim cacheShamanUser As String = ConfigurationManager.AppSettings("CacheShamanUser").ToString()
            Dim cacheCentroOperativo As Integer = Val(ConfigurationManager.AppSettings("CacheCentroOperativo").ToString())
            IsResend = setIntToBool(Val(ConfigurationManager.AppSettings("Resend").ToString()))

            If ShamanSession.Iniciar(cacheServer, cachePort, cacheNameSpace, cacheApp, cacheShamanUser, cacheCentroOperativo) Then

                vEmailsTop = Val(My.Resources.EmailTop)
                configShaman = New ShamanClases.PanelC.GlobalParameters

                Me.CreateDT()
                Me.tmrInit.Enabled = True

            End If

        Catch ex As Exception
            HandleError(Me.Name, "Main_Load", ex)
        End Try
    End Sub

    Private Sub CreateDT()
        Try
            dtLog.Columns.Add("ID", GetType(Decimal))
            dtLog.Columns.Add("Comprobante", GetType(String))
            dtLog.Columns.Add("Destino", GetType(String))
            dtLog.Columns.Add("Observaciones", GetType(String))
            Me.grdLog.DataSource = dtLog
        Catch ex As Exception
            HandleError(Me.Name, "CreateDT", ex)
        End Try
    End Sub

    Private Sub EnviarMails()
        Try

            Dim objMails As New ShamanClases.PanelC.MensajesPager
            Dim objAutomatizacion As New VentasDX.CliDocAutomatizacion
            Dim objClientesDocumentos As New ShamanClases.VentasC.ClientesDocumentos
            Dim objEmpresas As New Rrhh.EmpresasLegales
            Dim objMensajeria As New Mensajeria

            '------------> Facturación Pendiente
            '------------> Facturación Pendiente

            stLog.Caption = "Buscando mails de facturación"
            stLog.Refresh()

            Dim dt As DataTable = objMails.GetMailsFacturacionPendientes(vEmailsTop)
            Dim vIdx As Integer

            stLog.Caption = "Se encontraron " & dt.Rows.Count & " facturas para enviar"
            stLog.Refresh()

            For vIdx = 0 To dt.Rows.Count - 1

                Dim vMsgRef As String = ""

                If objClientesDocumentos.Abrir(dt.Rows(vIdx)("ClienteDocumentoId")) Then

                    stLog.Caption = "Preparando " & objClientesDocumentos.NroComprobante
                    stLog.Refresh()

                    Debug.Print(objClientesDocumentos.TalonarioId.ID)
                    Debug.Print(objClientesDocumentos.TalonarioId.EmpresaLegalId.ID)
                    Debug.Print(objClientesDocumentos.NroComprobante)

                    If IsResend AndAlso Now.Date > New Date(2023, 2, 15) Then
                        IsResend = False
                    End If

                    '-----> Envio Email
                    Dim vSend As Boolean = objAutomatizacion.SendComprobanteEmail(objClientesDocumentos.ID, objClientesDocumentos.ClienteId.ID, objClientesDocumentos.TalonarioId.EmpresaLegalId.RazonSocial,
                                                                                  objClientesDocumentos.NroComprobante, setTipoCliente(objClientesDocumentos.ClienteId.TipoClienteId.TipoClienteId), objClientesDocumentos.TipoProceso,
                                                                                  vMsgRef, , , IsResend)

                    If vSend Then
                        objMails.SetEstadoMensaje(dt.Rows(vIdx)("ID"), setBoolToInt(vSend), "")
                    Else
                        objMails.SetEstadoMensaje(dt.Rows(vIdx)("ID"), setBoolToInt(vSend), vMsgRef)
                    End If

                    AddLog(objClientesDocumentos.ID, objClientesDocumentos.NroComprobante, dt.Rows(vIdx)("Mensaje"), vMsgRef)

                    stLog.Caption = "Enviado " & objClientesDocumentos.NroComprobante
                    stLog.Refresh()

                Else

                    objMails.Eliminar(dt.Rows(vIdx)("ID"))

                End If

                Me.grdLog.RefreshDataSource()

            Next vIdx

            '------------> Notificaciones a Clientes
            '------------> Notificaciones a Clientes

            If dt.Rows.Count < vEmailsTop Then

                '----> Preparo path adjuntos
                Dim oPath As New PanelC.PathsConfiguracion()
                Dim pathAdjunto As String = oPath.GetPath("MDJ")
                If pathAdjunto = "" Then
                    pathAdjunto = "\\Archivos02\shamanadj$\mailing"
                End If

                stLog.Caption = "Buscando mails para clientes"
                stLog.Refresh()

                dt = objMails.GetMailsHTMLPendientes(vEmailsTop - dt.Rows.Count)

                stLog.Caption = "Se encontraron " & dt.Rows.Count & " notificaciones clientes para enviar"
                stLog.Refresh()

                For vIdx = 0 To dt.Rows.Count - 1


                    If objMails.Abrir(dt.Rows(vIdx)("ID")) Then

                        If objMails.rtfMensaje.RTF <> "" Then

                            Dim file1 As String = ""
                            Dim file2 As String = ""

                            If objMails.Mensaje <> "" Then

                                Dim adjuntos As String() = objMails.Mensaje.Split(";")

                                If adjuntos.Length > 0 Then

                                    file1 = pathAdjunto & "\" & adjuntos(0)

                                    If adjuntos.Length > 1 Then
                                        file2 = pathAdjunto & "\" & adjuntos(1)
                                    End If

                                End If

                            End If

                            '-----> Envio Email HTML
                            Dim dtEnvios As DataTable = objMensajeria.SendMail(objMails.Email, objMails.Titulo, objMails.rtfMensaje.RTF, file1, file2, "Facturacion", True)

                            Dim vSend As Boolean = False
                            Dim vMsgRef As String = ""
                            Dim vEnv As Integer = 0

                            For vEnv = 0 To dtEnvios.Rows.Count - 1

                                If dtEnvios.Rows(vEnv)("Enviado") Then
                                    vSend = True
                                Else
                                    vMsgRef = dtEnvios.Rows(vEnv)("Error")
                                End If

                            Next

                            If vSend Then
                                objMails.SetEstadoMensaje(dt.Rows(vIdx)("ID"), setBoolToInt(vSend), "")
                            Else
                                objMails.SetEstadoMensaje(dt.Rows(vIdx)("ID"), setBoolToInt(vSend), vMsgRef)
                            End If


                            AddLog(0, objMails.Titulo, objMails.Email, vMsgRef)

                        Else

                            objMails.Eliminar(dt.Rows(vIdx)("ID"))

                        End If

                    Else

                        objMails.Eliminar(dt.Rows(vIdx)("ID"))

                    End If


                Next vIdx

            End If

            '------------> Notificaciones a Médicos
            '------------> Notificaciones a Médicos

            stLog.Caption = "Buscando mails area médica / prestadores"
            stLog.Refresh()

            Dim objNotif As New ShamanClases.ExtranetC.Notificaciones
            Dim objNotifUser As New ShamanClases.ExtranetC.NotificacionesUsuarios

            dt = objNotif.GetPendientesEmail(vEmailsTop - dt.Rows.Count)

            stLog.Caption = "Se encontraron " & dt.Rows.Count & " notificaciones médicas / prestadores"
            stLog.Refresh()

            For vIdx = 0 To dt.Rows.Count - 1

                Dim objNotificacion As New ShamanClases.ExtranetC.Notificaciones

                If objNotificacion.Abrir(dt.Rows(vIdx)("NotificacionId")) Then

                    If objNotificacion.rtfMensaje.RTF <> "" Then

                        '-----> Envio Email Direccion Médica
                        Dim dtEnvios As DataTable

                        Select Case dt.Rows(vIdx)("Sender")
                            Case 1 : dtEnvios = objMensajeria.SendMail(dt.Rows(vIdx)("Email"), "Comunicado Red de Prestadores Grupo Paramedic", objNotificacion.rtfMensaje.RTF, , , "Prestadores", True)
                            Case Else : dtEnvios = objMensajeria.SendMail(dt.Rows(vIdx)("Email"), "Comunicado Dirección Médica Grupo Paramedic", objNotificacion.rtfMensaje.RTF, , , "DireccionMedica", True)
                        End Select

                        Dim vSend As Boolean = False
                        Dim vMsgRef As String = ""
                        Dim vEnv As Integer = 0

                        For vEnv = 0 To dtEnvios.Rows.Count - 1

                            If dtEnvios.Rows(vEnv)("Enviado") Then
                                vSend = True
                            Else
                                vMsgRef = dtEnvios.Rows(vEnv)("Error")
                            End If

                        Next

                        If vSend Then
                            objNotifUser.SetEstadoMensajeEmail(dt.Rows(vIdx)("ID"), setBoolToInt(vSend), "")
                        Else
                            objNotifUser.SetEstadoMensajeEmail(dt.Rows(vIdx)("ID"), setBoolToInt(vSend), vMsgRef)
                        End If

                        Select Case dt.Rows(vIdx)("Sender")
                            Case 1 : AddLog(0, "Prestadores", dt.Rows(vIdx)("Email"), vMsgRef)
                            Case Else : AddLog(0, "Dirección Médica", dt.Rows(vIdx)("Email"), vMsgRef)
                        End Select

                    End If

                End If


            Next vIdx

        Catch ex As Exception
            HandleError(Me.Name, "EnviarMails", ex)
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub FacturarCopagos()
        Try

            Dim objClientesDocumentos As New ShamanClases.VentasC.ClientesDocumentos

            '------------> Facturación Pendiente
            '------------> Facturación Pendiente

            stLog.Caption = "Buscando copagos pendientes de de facturación"
            stLog.Refresh()

            Dim dt As DataTable = objClientesDocumentos.GetCopagosPendientesFacturacion(Now.Date.AddDays(-10), Now.Date)
            Dim vIdx As Integer

            stLog.Caption = "Se encontraron " & dt.Rows.Count & " facturas para emitir"
            stLog.Refresh()

            If dt.Rows.Count > 0 Then

                Dim objTalonarios As New ShamanClases.VentasC.Talonarios()
                Dim objTalonariosCMS As New ShamanClases.VentasC.TalonariosCMS()

                If objTalonarios.Abrir(objTalonarios.GetCopagoDefault()) Then

                    Dim vNroSig As Long = 0

                    If objTalonarios.flgAFIPCae = 1 Then

                        If Not objTalonariosCMS.Abrir(objTalonariosCMS.GetCurrentId(objTalonarios.EmpresaLegalId.ID)) Then

                            stLog.Caption = "No hay CAE del día"
                            stLog.Refresh()

                            Exit Sub

                        End If

                        vNroSig = Me.ObtenerProximoNro(objTalonarios, objTalonariosCMS)

                    Else

                        vNroSig = objTalonarios.numActual

                    End If

                    If vNroSig > 0 Then

                        For vIdx = 0 To dt.Rows.Count - 1

                            Dim row As DataRow = dt.Rows(vIdx)

                            Dim dtRenglones As DataTable = Me.createDTRenglones()

                            Dim rowRen As DataRow = dtRenglones.NewRow()

                            rowRen("NroRenglon") = 1
                            rowRen("TipoRenglon") = 0
                            rowRen("Detalle") = String.Format("Servicio prestado el {0} en concepto {1}", CDate(row("FecIncidente")).ToShortDateString(), row("GradoOperativo"))
                            rowRen("ConceptoId") = "CPM"
                            rowRen("Unitario") = row("Copago")
                            rowRen("Cantidad") = 1
                            rowRen("Importe") = row("Copago")
                            rowRen("AlicuotaIvaId") = row("AlicuotaIvaId")
                            rowRen("Iva") = 0
                            rowRen("AlicuotaArbaId") = 0
                            rowRen("Arba") = 0
                            rowRen("AlicuotaAgipId") = 0
                            rowRen("Agip") = 0
                            rowRen("ArbaGravado") = 0
                            rowRen("AgipGravado") = 0
                            rowRen("IncidenteId") = row("ID")
                            rowRen("Incidente") = String.Format("{0} - {1}", CDate(row("FecIncidente")).ToShortDateString(), row("NroIncidente"))
                            rowRen("OtroProductoId") = 0
                            rowRen("AjusteImpuestoId") = 0
                            rowRen("Zonas") = ""

                            dtRenglones.Rows.Add(rowRen)

                            Dim vDocIdId As Decimal = objClientesDocumentos.jobFacturacionSimple("FAC", objTalonarios.ID, vNroSig, 0, row("ClienteId"), Now.Date, Now.Date, Val(Now.Date.Year & Now.Date.Month.ToString("00")), "CMP", 0, 1, "D", "D", dtRenglones, , , , Val(row("NroDocumento")))

                            '---> AFIP

                            If objClientesDocumentos.Abrir(vDocIdId) Then

                                Dim vDevVal As DevValidacion = Me.SendToAFIP(objClientesDocumentos, objTalonariosCMS, objTalonarios)

                                '---> Mailing

                                If vDevVal.Resultado Then

                                    Dim objAutomatizacion As New VentasDX.CliDocAutomatizacion()

                                    Dim vMsgRef As String = ""

                                    Dim vSend As Boolean = objAutomatizacion.SendComprobanteEmail(objClientesDocumentos.ID, objClientesDocumentos.ClienteId.ID, objTalonarios.EmpresaLegalId.RazonSocial,
                                                                                   objClientesDocumentos.NroComprobante, hTipoCliente.hDirectos, "CMP", vMsgRef, False, True, False, row("Email"))

                                    If vSend Then

                                        AddLog(objClientesDocumentos.ID, objClientesDocumentos.NroComprobante, row("Email"), "")

                                    Else

                                        AddLog(objClientesDocumentos.ID, objClientesDocumentos.NroComprobante, row("Email"), vMsgRef)

                                    End If

                                Else

                                    AddLog(objClientesDocumentos.ID, objClientesDocumentos.NroComprobante, row("Email"), vDevVal.DescripcionError)

                                End If

                            Else

                                AddLog(objClientesDocumentos.ID, objClientesDocumentos.NroComprobante, row("Email"), "Errores en Shaman al emitir el comprobante")

                            End If

                        Next vIdx

                    End If

                Else

                    stLog.Caption = "No hay talonario para copagos"
                    stLog.Refresh()

                End If


            End If

            objClientesDocumentos = Nothing

        Catch ex As Exception
            HandleError(Me.Name, "EnviarMails", ex)
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function ObtenerProximoNro(ByVal objTalonarios As ShamanClases.VentasC.Talonarios, ByVal objTalonariosCMS As ShamanClases.VentasC.TalonariosCMS) As Long

        ObtenerProximoNro = 0

        Try

            Me.Cursor = Cursors.WaitCursor

            Dim FEAuthRequest As New wsfev1Produccion.FEAuthRequest
            Dim objWSFEV1 As New wsfev1Produccion.ServiceSoapClient

            FEAuthRequest.Token = objTalonariosCMS.Token.RTF
            FEAuthRequest.Sign = objTalonariosCMS.Sign
            FEAuthRequest.Cuit = Val(objTalonarios.EmpresaLegalId.CUIT.Replace("-", ""))

            'Dim PtoVta As Long = ListBox_FECompUltimoAutorizado_PtoVta.SelectedItem.ToString.Substring(0, 2)
            'Dim CbteTipo As Long = ListBox_FECompUltimoAutorizado_CbteTipo.SelectedItem.ToString.Substring(0, 2)
            Dim objFERecuperaLastCbteResponse As wsfev1Produccion.FERecuperaLastCbteResponse

            ' Invoco al método FECompUltimoAutorizado
            Try
                objFERecuperaLastCbteResponse = objWSFEV1.FECompUltimoAutorizado(FEAuthRequest, objTalonarios.Sucursal, getCbteTipo(objTalonarios.TipoComprobanteAfip, objTalonarios.TipoDocumento.TipoDocumentoId, objTalonarios.Letra))
                If objFERecuperaLastCbteResponse IsNot Nothing Then
                    ObtenerProximoNro = objFERecuperaLastCbteResponse.CbteNro + 1
                End If
            Catch ex As Exception

            End Try

            Me.Cursor = Cursors.Default

        Catch ex As Exception
            HandleError(Me.Name, "ObtenerProximoNro", ex)
        End Try
    End Function

    Private Function createDTRenglones() As DataTable
        Try

            Dim dtRenglones As New DataTable

            dtRenglones.Columns.Add("NroRenglon", GetType(Integer))
            dtRenglones.Columns.Add("TipoRenglon", GetType(Integer))
            dtRenglones.Columns.Add("Detalle", GetType(String))
            dtRenglones.Columns.Add("ConceptoId", GetType(String))
            dtRenglones.Columns.Add("Unitario", GetType(Decimal))
            dtRenglones.Columns.Add("Cantidad", GetType(Decimal))
            dtRenglones.Columns.Add("Importe", GetType(Decimal))
            dtRenglones.Columns.Add("AlicuotaIvaId", GetType(Decimal))
            dtRenglones.Columns.Add("Iva", GetType(Decimal))
            dtRenglones.Columns.Add("AlicuotaArbaId", GetType(Decimal))
            dtRenglones.Columns.Add("Arba", GetType(Decimal))
            dtRenglones.Columns.Add("AlicuotaAgipId", GetType(Decimal))
            dtRenglones.Columns.Add("Agip", GetType(Decimal))
            dtRenglones.Columns.Add("ArbaGravado", GetType(Decimal))
            dtRenglones.Columns.Add("AgipGravado", GetType(Decimal))
            dtRenglones.Columns.Add("IncidenteId", GetType(Decimal))
            dtRenglones.Columns.Add("Incidente", GetType(String))
            dtRenglones.Columns.Add("OtroProductoId", GetType(Decimal))
            dtRenglones.Columns.Add("AjusteImpuestoId", GetType(Integer))
            dtRenglones.Columns.Add("Zonas", GetType(String))

            createDTRenglones = dtRenglones

        Catch ex As Exception

            createDTRenglones = Nothing

        End Try
    End Function

    Private Function SendToAFIP(objDocumento As ShamanClases.VentasC.ClientesDocumentos, objTalonariosCMS As ShamanClases.VentasC.TalonariosCMS, objTalonarios As ShamanClases.VentasC.Talonarios) As DevValidacion

        SendToAFIP = New DevValidacion("Sin procesar")

        Try

            Dim objWSFEV1 As New wsfev1Produccion.ServiceSoapClient

            Dim FEAuthRequest As New wsfev1Produccion.FEAuthRequest

            FEAuthRequest.Token = objTalonariosCMS.Token.RTF
            FEAuthRequest.Sign = objTalonariosCMS.Sign
            FEAuthRequest.Cuit = Val(objTalonarios.EmpresaLegalId.CUIT.Replace("-", ""))

            Dim objFECAECabRequest As New wsfev1Produccion.FECAECabRequest
            Dim objFECAERequest As New wsfev1Produccion.FECAERequest
            Dim objFECAEResponse As New wsfev1Produccion.FECAEResponse

            Dim indicemax_arrayFECAEDetRequest As Integer = 0
            Dim d_arrayFECAEDetRequest As Integer = 0
            Dim arrayFECAEDetRequest(indicemax_arrayFECAEDetRequest) As wsfev1Produccion.FECAEDetRequest

            Dim vNroCae As Decimal = 0
            Dim vCaeVto As String = ""

            objFECAECabRequest.CantReg = 1
            objFECAECabRequest.CbteTipo = getCbteTipo(objDocumento.TalonarioId.TipoComprobanteAfip, objDocumento.TipoComprobante.TipoDocumentoId, objDocumento.NroComprobante.Substring(0, 1))

            '-----> Verificar si es la sucursal
            objFECAECabRequest.PtoVta = objDocumento.TalonarioId.Sucursal

            Dim objFECAEDetRequest As New wsfev1Produccion.FECAEDetRequest

            With objFECAEDetRequest
                .Concepto = 2

                Dim vCuit As String = objDocumento.CUIT

                vCuit = vCuit.Replace("_", "0")
                If vCuit.Length = 13 Then
                    If vCuit.Substring(0, 2) = "00" Then
                        vCuit = vCuit.Substring(3, 8)
                    End If
                End If
                vCuit = vCuit.Replace("-", "")

                Select Case Val(vCuit).ToString.Length
                    Case 7, 8 : .DocTipo = 96
                    Case 11 : .DocTipo = 80
                    Case Else : .DocTipo = 99
                End Select

                .DocNro = Val(vCuit)

                .CbteDesde = objDocumento.NumeroId
                .CbteHasta = objDocumento.NumeroId
                .CbteFch = DtoN(objDocumento.FecDocumento)
                .ImpTotal = objDocumento.Importe
                .ImpTotConc = 0
                .ImpNeto = objDocumento.ImporteGravado
                .ImpOpEx = objDocumento.ImporteExento
                .ImpTrib = objDocumento.ImporteIIBB + objDocumento.ImporteAGIP + objDocumento.ImporteIvaNoInscripto
                .ImpIVA = objDocumento.ImporteIvaInscripto

                .FchServDesde = Val(objDocumento.Periodo.ToString & "01")
                .FchServHasta = Val(objDocumento.Periodo.ToString & GetLastDayMonth(GetPeriodo(objDocumento.Periodo)).ToString)

                .FchVtoPago = DtoN(objDocumento.FecVencimiento)

                .MonId = "PES"
                .MonCotiz = 1

                .CondicionIVAReceptorId = Val(objDocumento.SituacionIva.CodigoAfip)

            End With

            '------> Iva
            Dim vCntIva As Integer = 0

            If objDocumento.ImporteIvaInscripto > 0 Then
                vCntIva += 1
            End If

            If vCntIva > 0 Then

                Dim objIvaTipo As wsfev1Produccion.IvaTipoResponse
                objIvaTipo = objWSFEV1.FEParamGetTiposIva(FEAuthRequest)

                Dim objIvaDoc(vCntIva - 1) As wsfev1Produccion.AlicIva
                Dim vIdxIva As Integer = 0

                If objDocumento.ImporteIvaInscripto > 0 Then

                    objIvaDoc(vIdxIva) = New wsfev1Produccion.AlicIva
                    objIvaDoc(vIdxIva).Id = Me.afipIvaId(objIvaTipo, objDocumento.PorcIvaInscripto)
                    objIvaDoc(vIdxIva).BaseImp = objDocumento.ImporteGravado
                    objIvaDoc(vIdxIva).Importe = objDocumento.ImporteIvaInscripto
                    vIdxIva = vIdxIva + 1

                End If

                '------> Iva
                objFECAEDetRequest.Iva = objIvaDoc

            End If

            '----> IIBB
            Dim vCntTri As Integer = 0

            If objDocumento.ImporteIIBB > 0 Then
                vCntTri = vCntTri + 1
            End If
            If objDocumento.ImporteAGIP > 0 Then
                vCntTri = vCntTri + 1
            End If
            If objDocumento.ImporteIvaNoInscripto > 0 Then
                vCntTri = vCntTri + 1
            End If


            If vCntTri > 0 Then

                Dim objTributoTipo As wsfev1Produccion.FETributoResponse
                objTributoTipo = objWSFEV1.FEParamGetTiposTributos(FEAuthRequest)

                Dim objTributoDoc(vCntTri - 1) As wsfev1Produccion.Tributo
                Dim vIdxTri As Integer = 0

                If objDocumento.ImporteIIBB > 0 Then

                    objTributoDoc(vIdxTri) = New wsfev1Produccion.Tributo
                    objTributoDoc(vIdxTri).Id = Me.afipTributoId(objTributoTipo, objDocumento.PorcentajeIIBB)
                    objTributoDoc(vIdxTri).Desc = "IIBB ARBA " & objDocumento.PorcentajeIIBB & " %"
                    objTributoDoc(vIdxTri).BaseImp = objDocumento.ImpGravadoIIBB
                    objTributoDoc(vIdxTri).Importe = objDocumento.ImporteIIBB
                    vIdxTri = vIdxTri + 1

                End If

                If objDocumento.ImporteAGIP > 0 Then

                    objTributoDoc(vIdxTri) = New wsfev1Produccion.Tributo
                    objTributoDoc(vIdxTri).Id = Me.afipTributoId(objTributoTipo, objDocumento.PorcentajeAGIP)
                    objTributoDoc(vIdxTri).Desc = "IIBB AGIP " & objDocumento.PorcentajeAGIP & " %"
                    objTributoDoc(vIdxTri).BaseImp = objDocumento.ImpGravadoAGIP
                    objTributoDoc(vIdxTri).Importe = objDocumento.ImporteAGIP
                    vIdxTri = vIdxTri + 1

                End If

                If objDocumento.ImporteIvaNoInscripto > 0 Then

                    objTributoDoc(vIdxTri) = New wsfev1Produccion.Tributo
                    objTributoDoc(vIdxTri).Id = Me.afipTributoId(objTributoTipo, objDocumento.PorcIvaNoInscripto)
                    objTributoDoc(vIdxTri).Desc = "IVA NO CATEG. " & objDocumento.PorcIvaNoInscripto & " %"
                    objTributoDoc(vIdxTri).BaseImp = objDocumento.ImporteGravado
                    objTributoDoc(vIdxTri).Importe = objDocumento.ImporteIvaNoInscripto
                    vIdxTri = vIdxTri + 1

                End If

                '------> IIBB
                objFECAEDetRequest.Tributos = objTributoDoc

            End If

            arrayFECAEDetRequest(d_arrayFECAEDetRequest) = objFECAEDetRequest

            With objFECAERequest
                .FeCabReq = objFECAECabRequest
                .FeDetReq = arrayFECAEDetRequest
            End With

            ' Invoco al método FECAESolicitar

            Try

                objFECAEResponse = objWSFEV1.FECAESolicitar(FEAuthRequest, objFECAERequest)

                'If objFECAERequest IsNot Nothing Then
                '    ''Serialize object to a text file.
                '    Dim objStreamWriter As New StreamWriter(My.Application.Info.DirectoryPath & "\WSFEV1_objFECAERequest.xml")
                '    Dim x As New XmlSerializer(objFECAERequest.GetType)
                '    x.Serialize(objStreamWriter, objFECAERequest)
                '    objStreamWriter.Close()
                'End If

                'If objFECAEResponse IsNot Nothing Then
                '    ''Serialize object to a text file.
                '    Dim objStreamWriter As New StreamWriter(My.Application.Info.DirectoryPath & "\WSFEV1_objFECAEResponse.xml")
                '    Dim x As New XmlSerializer(objFECAEResponse.GetType)
                '    x.Serialize(objStreamWriter, objFECAEResponse)
                '    objStreamWriter.Close()
                'End If

                If objFECAEResponse.Errors IsNot Nothing Then

                    For i = 0 To objFECAEResponse.Errors.Length - 1

                        Return New DevValidacion(objFECAEResponse.Errors(i).Code.ToString & ": " & objFECAEResponse.Errors(i).Msg)

                    Next

                End If

                If objFECAEResponse.FeDetResp IsNot Nothing Then

                    Dim vHavErr As Boolean = False
                    Dim vErrDes As String = ""

                    For i = 0 To objFECAEResponse.FeDetResp.Length - 1

                        If objFECAEResponse.FeDetResp(i).Resultado = "A" Then
                            vNroCae = objFECAEResponse.FeDetResp(i).CAE
                            vCaeVto = objFECAEResponse.FeDetResp(i).CAEFchVto.ToString
                        Else

                            vHavErr = True

                        End If

                        If objFECAEResponse.FeDetResp(i).Observaciones IsNot Nothing Then

                            Dim j As Integer

                            For j = 0 To objFECAEResponse.FeDetResp(i).Observaciones.Length - 1

                                vErrDes = objFECAEResponse.FeDetResp(i).Observaciones(j).Code.ToString & ": " & objFECAEResponse.FeDetResp(i).Observaciones(j).Msg

                            Next

                            If vHavErr Then

                                Return New DevValidacion(vErrDes)

                            End If

                        End If

                    Next

                End If

                '-----> Analizo último comprobante OK

                objDocumento.SetNroCAE(objDocumento.ID, vNroCae, vCaeVto, objDocumento.GetCodigoBarra(FEAuthRequest.Cuit.ToString, objFECAECabRequest.CbteTipo, objFECAECabRequest.PtoVta, vNroCae.ToString, vCaeVto))

                Return New DevValidacion()

            Catch ex As Exception

                HandleError(Me.Name, "SendToAFIP", ex)

            End Try

        Catch ex As Exception

            HandleError(Me.Name, "SendToAFIP", ex)

        End Try
    End Function

    Private Function afipIvaId(objIvaTipo As wsfev1Produccion.IvaTipoResponse, pPor As Decimal) As Integer
        afipIvaId = 0
        Try
            Dim vFnd As Boolean = False
            Dim i As Integer = 0

            Do Until vFnd Or i = objIvaTipo.ResultGet.Length

                Dim vPorVal As String = objIvaTipo.ResultGet(i).Desc.Replace("%", "")
                vPorVal = vPorVal.Replace(".", wSepDecimal)

                If CDbl(vPorVal) = pPor Then
                    vFnd = True
                Else
                    i = i + 1
                End If

            Loop

            If vFnd Then
                afipIvaId = objIvaTipo.ResultGet(i).Id
            End If

        Catch ex As Exception
            HandleError(Me.Name, "afipIvaId", ex)
        End Try
    End Function

    Private Function afipTributoId(objTributoTipo As wsfev1Produccion.FETributoResponse, pPor As Decimal) As Integer
        afipTributoId = 0
        Try
            Dim vFnd As Boolean = False
            Dim i As Integer = 0

            Do Until vFnd Or i = objTributoTipo.ResultGet.Length

                Dim vPorVal As String = objTributoTipo.ResultGet(i).Desc.Replace("%", "")
                vPorVal = vPorVal.Replace(".", wSepDecimal)

                i = i + 1

            Loop

            If vFnd Then
                afipTributoId = objTributoTipo.ResultGet(i).Id
            Else
                afipTributoId = objTributoTipo.ResultGet(i - 1).Id
            End If

        Catch ex As Exception
            HandleError(Me.Name, "afipTributoId", ex)
        End Try
    End Function

    Private Sub AddLog(pId As Decimal, pCmp As String, pDst As String, pErrMsg As String)
        Try

            Dim dtRow As DataRow = dtLog.NewRow

            dtRow("ID") = pId
            dtRow("Comprobante") = pCmp
            dtRow("Destino") = pDst
            If pErrMsg = "" Then
                dtRow("Observaciones") = "OK"
            Else
                dtRow("Observaciones") = pErrMsg
            End If

            dtLog.Rows.Add(dtRow)

        Catch ex As Exception
            HandleError(Me.Name, "AddLog", ex)
        End Try
    End Sub

    Private Sub btnCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick
        Try
            Me.Close()
        Catch ex As Exception
            HandleError(Me.Name, "btnCerrar_ItemClick", ex)
        End Try
    End Sub

    Private Sub tmrInit_Tick(sender As Object, e As EventArgs) Handles tmrInit.Tick
        Try
            Me.tmrInit.Enabled = False
            'Me.EnviarMails()
            Me.FacturarCopagos()
            Me.tmrInit.Enabled = True
        Catch ex As Exception
            HandleError(Me.Name, "tmrInit_Tick", ex)
        End Try
    End Sub

End Class
