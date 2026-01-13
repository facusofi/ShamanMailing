Imports InterSystems.Data.CacheClient
Imports InterSystems.Data.CacheTypes
Imports System.IO
Imports ShamanClases_CSharp
Imports ShamanClases


Namespace VentasDX
    Public Class CliDocAutomatizacion
        Inherits ShamanClases.VentasC.CliDocAutomatizacion
        Public Sub New(Optional ByVal pCnnSta As String = "", Optional ByVal pCnnDyn As String = "")
            MyBase.New(pCnnSta, pCnnDyn)
        End Sub
        Public Function SendComprobanteEmail(ByVal pDoc As Decimal, ByVal pCli As Decimal, ByVal pEmp As String, ByVal pNro As String, ByVal pTcl As hTipoCliente, ByVal pTPro As String, ByRef pMsgRef As String, Optional ByVal pMsg As Boolean = False, Optional ByVal pGoQueue As Boolean = False, Optional ByVal pIsResend As Boolean = False, Optional ByVal pEmail As String = "") As Boolean

            SendComprobanteEmail = False
            pMsgRef = ""

            Try

                Dim objContactos As New VentasC.ClientesTelefonosMails
                Dim objMensajeria As New Mensajeria
                Dim vMsgDst As String = pEmail

                If vMsgDst = "" Then

                    Dim dtMails As DataTable = objContactos.GetDTMails(pCli, hTelefonosMails.hMailingFacturacion)

                    If dtMails.Rows.Count > 0 Then

                        Dim vMai As Integer = 0

                        '----> Mails separados por ;
                        For vMai = 0 To dtMails.Rows.Count - 1

                            Dim vMail As String = dtMails.Rows(vMai)(1).ToString
                            vMail = vMail.Substring(InStr(vMail, "("), vMail.Length - InStr(vMail, "(") - 1).ToLower()

                            If vMsgDst = "" Then
                                vMsgDst = vMail
                            Else
                                vMsgDst = vMsgDst & ";" & vMail
                            End If

                        Next vMai

                    End If

                End If

                '-------> Cliente x Cliente los mails...

                If vMsgDst <> "" Then

                    '----> Envío

                    Dim vSend As Boolean = True

                    If pMsg Then
                        If MsgBox("¿ Desea enviar el comprobante a " & vMsgDst & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton1, "Mailing") = MsgBoxResult.No Then
                            vSend = False
                        End If
                    End If

                    If vSend Then

                        vSend = False

                        If Not pGoQueue Then

                            Dim vSubj As String = "Ud. ha recibido un nuevo comprobante de " & toUpperLower(pEmp)
                            Dim vBody As String = ""
                            Dim vFile As String = My.Application.Info.DirectoryPath & "\" & pNro & ".pdf"
                            Dim vFile2 As String = ""
                            Dim vFile3 As String = ""

                            vBody = "Adjuntamos en el presente correo nuestro comprobante " & pNro & " en formato PDF."
                            vBody += vbCrLf
                            vBody += "Muchas gracias" & vbCrLf
                            vBody += toUpperLower(pEmp) & vbCrLf

                            If pIsResend Then

                                vSubj = "Rectificación de Comprobante de " & toUpperLower(pEmp)

                                vBody = "De mi mayor consideración:"
                                vBody += vbCrLf
                                vBody += vbTab & "Reenviamos su factura " & pNro & " correspondiente al mes de Febrero de 2023, ya que la enviada anteriormente no contaba con el código de barras que da la posibilidad de abonarlas en PagoFácil"
                                vBody += vbCrLf
                                vBody += vbTab & "Desde ya muchas gracias y disculpas por las molestias ocasionadas"
                                vBody += vbTab & toUpperLower(pEmp) & vbCrLf

                            End If

                            If configShaman.insLicencia = hLicencia.hParamedic Then

                                '----> Extranet
                                'If pTPro = "PRE" Then

                                If pTcl = hTipoCliente.hDirectos Then

                                    If Not Me.haveUserExtranet(pCli) Then

                                        If File.Exists(My.Application.Info.DirectoryPath & "\Extranet.pdf") Then

                                            vFile2 = My.Application.Info.DirectoryPath & "\Extranet.pdf"

                                            vBody += vbCrLf
                                            vBody += vbCrLf
                                            vBody += " Adjuntamos instructivo para conocer el detalle de su cuenta corriente a través de nuestra web"

                                        End If

                                    End If

                                    If Now.Year = 2020 And Now.Month = 7 Then

                                        If File.Exists(My.Application.Info.DirectoryPath & "\NotaMinorista.docx") Then

                                            vFile3 = My.Application.Info.DirectoryPath & "\NotaMinorista.docx"

                                        End If

                                    End If

                                End If

                                '----> Forma de Pago

                                If pTcl <> hTipoCliente.hEntidades Then

                                    vBody += vbCrLf
                                    vBody += vbCrLf
                                    vBody += " Por tema PAGOS O TRANSFERENCIAS BANCARIAS, enviar mail Dto. Cobranzas – cobranzas@paramedic.com.ar"
                                    vBody += vbCrLf
                                    'vBody += " Se comunica a nuestros clientes que, nuestro Dto. de Cobranzas comenzó a operar con la nueva herramienta TANGO NEXO" & vbCrLf
                                    'vBody += " Por tal razón a partir del 29-10-2021, fueron invitados a Registrarse, para hacer uso de la misma." & vbCrLf
                                    'vBody += " TANGO NEXO, les brindará la opción de poder chequear su, deuda vencida y por vencer, además de poder imprimir " & vbCrLf
                                    'vBody += " los recibos de facturas canceladas. " & vbCrLf
                                    'vBody += " Por temas relacionados a este punto, enviar mail a cobranzas@paramedic.com.ar" & vbCrLf

                                End If

                            End If

                            Dim dtView As New DataView
                            Dim vStream As New System.IO.MemoryStream
                            Dim objClientesDocumentos As New ShamanClases.VentasC.ClientesDocumentos

                            objClientesDocumentos.prepareToPrint(ShamanSession.PID, pDoc, dtView, vStream)

                            objClientesDocumentos = Nothing

                            Dim objReport As New repImagenDocumento

                            objReport.DataSource = dtView
                            objReport.LoadLayout(vStream)

                            objReport.ExportToPdf(vFile)


                            If File.Exists(vFile) Then

                                '------> Envío...

                                Dim dtEnvios As DataTable = objMensajeria.SendMail(vMsgDst, vSubj, vBody, vFile, vFile2, "Facturacion", , , , vFile3, True)
                                Dim vEnv As Integer

                                For vEnv = 0 To dtEnvios.Rows.Count - 1

                                    Dim objAutomatizacion As New ShamanClases.VentasC.CliDocAutomatizacion

                                    objAutomatizacion.ClienteDocumentoId.SetObjectId(pDoc)
                                    objAutomatizacion.NroAutomatizacion = 21
                                    objAutomatizacion.flgStatus = setBoolToInt(dtEnvios.Rows(vEnv)("Enviado"))
                                    objAutomatizacion.Referencia1 = Left(vMsgDst, 200)
                                    objAutomatizacion.Referencia2 = Left(dtEnvios.Rows(vEnv)("Error"), 200)

                                    objAutomatizacion.Salvar(objAutomatizacion)

                                    If dtEnvios.Rows(vEnv)("Enviado") Then
                                        vSend = True
                                    Else
                                        pMsgRef = dtEnvios.Rows(vEnv)("Error")
                                    End If

                                Next

                                If vSend Then pMsgRef = ""

                                SendComprobanteEmail = vSend

                                If Not vSend And pMsg And pMsgRef <> "" Then
                                    MsgBox(pMsgRef, MsgBoxStyle.Critical, "Mailing")
                                Else
                                    pMsgRef = vMsgDst
                                End If

                            Else

                                If pMsg Then
                                    pMsgRef = "No se pudo generar el archivo PDF"
                                    MsgBox("No se pudo generar el archivo PDF", MsgBoxStyle.Critical, "Mailing")
                                End If

                            End If

                        Else

                            '-----> Meto en cola...
                            vSend = objMensajeria.SendMailtoQueue(pDoc, vMsgDst)

                        End If

                    End If

                Else

                    pMsgRef = "No hay destinatarios configurados"

                End If

            Catch ex As Exception
                HandleError(Me.CacheClassController, "SendMail", ex)
            End Try

        End Function

    End Class
End Namespace
