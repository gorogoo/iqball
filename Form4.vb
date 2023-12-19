Imports System.Data.SqlClient
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Drawing.Printing

Public Class Form4
    Dim WithEvents PD As New PrintDocument
    Dim PPD As New PrintPreviewDialog
    Sub kondisiAwal()
        Using Conn As New SqlConnection(stringConnection)
            Conn.Open()
            Using sda As New SqlDataAdapter("select id_laporan,tgl_transaksi,total_bayar from Tbl_laporan", Conn)
                Using ds As New DataSet
                    sda.Fill(ds, "Tbl_laporan")
                    DataGridView1.DataSource = (ds.Tables("Tbl_laporan"))

                End Using
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kondisiAwal()

    End Sub

    Private Sub TransaksiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransaksiToolStripMenuItem.Click
        Me.Visible = False
        Form3.Visible = True
    End Sub

    Private Sub KelolaMenuToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KelolaMenuToolStripMenuItem.Click
        Me.Visible = False
        Form2.Visible = True
    End Sub

    Private Sub LogoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogoutToolStripMenuItem.Click
        Me.Visible = False
        Form1.Visible = True
    End Sub

    Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select id_laporan,tgl_transaksi,total_bayar from Tbl_laporan where CONVERT(date,tgl_transaksi) between @from and @end order by Tgl_Transaksi desc", conn)
                cmd.Parameters.AddWithValue("@from", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@end", Format(DateTimePicker2.Value, "yyyy-MM-dd"))
                Using sda As New SqlDataAdapter(cmd)
                    Dim ds As New DataSet
                    sda.Fill(ds, "Tbl_laporan")
                    DataGridView1.DataSource = ds.Tables("Tbl_laporan")
                End Using
            End Using
            conn.Close()
        End Using
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs)
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select tgl_Transaksi,total_bayar from Tbl_laporan where CONVERT(date,tgl_transaksi) between @from and @end", conn)
                cmd.Parameters.AddWithValue("@from", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@end", Format(DateTimePicker2.Value, "yyyy-MM-dd"))
                Dim sdr As SqlDataReader = cmd.ExecuteReader

                sdr.Close()
            End Using
            conn.Close()
        End Using
    End Sub


    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim printDialog As New PrintDialog()
        If printDialog.ShowDialog() = DialogResult.OK Then
            PD.Print()
        End If
    End Sub

    Private Sub PD_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PD.PrintPage
        Dim FONTJUDUL As New Font("Times New Roman", 18, FontStyle.Bold)
        Dim FONTDATA As New Font("Times New Roman", 11, FontStyle.Regular)
        Dim FONTDA As New Font("Times New Roman", 11, FontStyle.Bold)
        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        Dim kiri As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center
        kiri.Alignment = StringAlignment.Near
        e.Graphics.DrawString("LAPORAN PENJUALAN ANGKRINGANKU", FONTJUDUL, Brushes.Black, 450, 50, tengah)



        e.Graphics.DrawLine(Pens.Black, 30, 115, 805, 115)

        e.Graphics.DrawLine(Pens.Black, 30, 115, 30, 330)
        e.Graphics.DrawLine(Pens.Black, 118, 115, 118, 330)
        e.Graphics.DrawLine(Pens.Black, 298, 115, 298, 355)
        e.Graphics.DrawLine(Pens.Black, 598, 115, 598, 355)
        e.Graphics.DrawLine(Pens.Black, 805, 115, 805, 355)

        e.Graphics.DrawString("NOMER", FONTDA, Brushes.Black, 40, 115, kiri)

        e.Graphics.DrawString("ID TRANSAKSI", FONTDA, Brushes.Black, 120, 115, kiri)

        e.Graphics.DrawString("TGL TRANSAKSI", FONTDA, Brushes.Black, 300, 115, kiri)

        e.Graphics.DrawString("TOTAL HARGA", FONTDA, Brushes.Black, 600, 115, kiri)

        e.Graphics.DrawLine(Pens.Black, 30, 135, 805, 135)
        Dim baris As Integer = 145
        Dim NOMOR As Integer = 1
        Dim jumlahBeli As Integer
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_laporan where CONVERT(date,tgl_transaksi) between @from and @end", conn)
                cmd.Parameters.AddWithValue("@from", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@end", Format(DateTimePicker2.Value, "yyyy-MM-dd"))
                Dim dr As SqlDataReader = cmd.ExecuteReader
                dr.Read()
                While dr.Read
                    e.Graphics.DrawString(NOMOR, FONTDATA, Brushes.Black, 55, baris, kiri)

                    e.Graphics.DrawString(dr("id_transaksi"), FONTDATA, Brushes.Black, 160, baris, kiri)

                    e.Graphics.DrawString(FormatDateTime(dr("tgl_transaksi")), FONTDATA, Brushes.Black, 300, baris, kiri)



                    e.Graphics.DrawString(FormatCurrency(dr("total_bayar")), FONTDATA, Brushes.Black, 600, baris, kiri)


                    NOMOR = NOMOR + 1

                    baris = baris + 20


                    jumlahBeli += dr("total_bayar")

                End While
                e.Graphics.DrawLine(Pens.Black, 30, baris + 5, 805, baris + 5)

                e.Graphics.DrawString("TOTAL : ", FONTDATA, Brushes.Black, 305, baris + 9, kiri)

                e.Graphics.DrawString(FormatCurrency(jumlahBeli), FONTDATA, Brushes.Black, 600, baris + 9, kiri)


                e.Graphics.DrawLine(Pens.Black, 300, baris + 30, 805, baris + 30)

                e.Graphics.DrawString("Mengetahui", FONTDATA, Brushes.Black, 700, 500, kanan)
                e.Graphics.DrawString("Kasir", FONTDATA, Brushes.Black, 662, 515, kanan)
                e.Graphics.DrawString("Iqbal Hanggoro", FONTDATA, Brushes.Black, 722, 595, kanan)

                dr.Close()
            End Using
        End Using
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub LaporanPenjualanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LaporanPenjualanToolStripMenuItem.Click

    End Sub
End Class