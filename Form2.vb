Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Drawing.Printing
Public Class Form2
    Public selectedValue As String
    Public stok As Integer
    Dim WithEvents PD As New PrintDocument
    Dim PPD As New PrintPreviewDialog
    Dim longpaper As Integer
    Sub kondisiAwal()
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using sda As New SqlDataAdapter("select Tbl_transaksi.id_transaksi,Tbl_menu.kode_menu,Tbl_menu.nama_menu,Tbl_menu.harga_satuan,Tbl_transaksi.qty,Tbl_transaksi.total_bayar,Tbl_transaksi.tgl_transaksi from Tbl_transaksi inner join Tbl_menu on Tbl_transaksi.kode_barang=Tbl_menu.kode_menu", conn)
                Using ds As New DataSet
                    sda.Fill(ds, "Tbl_transaksi")
                    DataGridView1.DataSource = ds.Tables("Tbl_transaksi")
                End Using
            End Using
            conn.Close()
        End Using
    End Sub

    Sub kosongkan()
        txtHarga.Text = ""

        cmbKode.Text = ""
        txtTotal.Text = ""
        DateTimePicker1.Text = ""
        txtQty.Text = ""
    End Sub
    Sub txtDisabled()
        txtHarga.Enabled = False

        txtQty.Enabled = False
        cmbKode.Enabled = False
        DateTimePicker1.Enabled = False
        txtTotal.Enabled = False
    End Sub
    Sub txtEnabled()
        txtHarga.Enabled = True

        txtQty.Enabled = True
        cmbKode.Enabled = True
        DateTimePicker1.Enabled = True
        txtTotal.Enabled = True
    End Sub
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DateTimePicker1.Enabled = False
        kondisiAwal()
        txtDisabled()
        cmbdata()
    End Sub
    Sub rumusTotal()
        Dim total As Decimal
        For Each row As DataGridViewRow In DataGridView1.Rows

            total += row.Cells("total_bayar").Value
        Next
        labelTotal.Text = FormatCurrency(total)
    End Sub

    Private Sub KelolaMenuToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KelolaMenuToolStripMenuItem.Click

    End Sub

    Private Sub LogoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogoutToolStripMenuItem.Click
        Me.Visible = False
        Form1.Visible = True
    End Sub
    Private Sub txtQty_TextChanged(sender As Object, e As EventArgs) Handles txtQty.TextChanged
        Dim total, harga As Double
        If Decimal.TryParse(txtHarga.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, total) Then
            'harga = Decimal.Parse(txtHarga.Text.Replace("Rp", "").Replace(".", "").Replace(",00", ""))
            harga = CDbl(txtHarga.Text)
            total = harga * Val(txtQty.Text)
            txtTotal.Text = FormatCurrency(total)
        End If
    End Sub
    Private Sub cmbKode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbKode.SelectedIndexChanged
        selectedValue = cmbKode.SelectedItem
        Dim kd As String = selectedValue

        Using Conn As New SqlConnection(stringConnection)
            Conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_menu where kode_menu=@id", Conn)
                cmd.Parameters.AddWithValue("@id", kd)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                While sdr.Read()
                    txtId.Text = sdr("kode_menu")
                    txtHarga.Text = sdr("harga_satuan")
                End While
                sdr.Close()
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        If btnTambah.Text = "Tambah" Then
            btnTambah.Text = "Simpan"
            kosongkan()
            txtEnabled()
        Else
            Dim kd As String = selectedValue
            'cekStok()
            Using Conn As New SqlConnection(stringConnection)
                Dim countTr As Integer = 0
                Dim totalBayar As Double
                Conn.Open()
                'Using cmd As New SqlCommand("select COUNT(*) from Tbl_transaksi where kode_menu = @kd", Conn)
                '    cmd.Parameters.AddWithValue("@kd", kd)
                '    countTr = CInt(cmd.ExecuteScalar)
                'End Using
                Using cmd As New SqlCommand("select total_bayar from Tbl_transaksi where kode_barang = @kd", Conn)
                    cmd.Parameters.AddWithValue("@kd", kd)
                    totalBayar = CDbl(cmd.ExecuteScalar())
                End Using

                If countTr > 0 Then
                    Using cmd As New SqlCommand("update Tbl_transaksi set qty=qty+@qty,total_bayar= @subtotal where kode_barang=@kd", Conn)
                        If Not String.IsNullOrEmpty(txtTotal.Text) Then
                            cmd.Parameters.AddWithValue("@qty", txtQty.Text)
                            cmd.Parameters.AddWithValue("@subtotal", FormatCurrency(totalBayar + CDbl(txtTotal.Text)))
                            cmd.Parameters.AddWithValue("@kd", kd)
                            cmd.ExecuteNonQuery()
                            'updateStok()
                            kondisiAwal()
                            btnTambah.Text = "Tambah"
                            kosongkan()
                            rumusTotal()
                        Else
                            MsgBox("Total tidak valid")
                            kosongkan()
                        End If
                    End Using
                Else
                    Using cmd As New SqlCommand("insert into Tbl_transaksi (no_transaksi,tgl_transaksi,total_bayar,qty,kode_barang) values (@notran,@tgl,@subtotal,@qty,@kd)", Conn)
                        cmd.Parameters.AddWithValue("@qty", txtQty.Text)
                        cmd.Parameters.AddWithValue("@subtotal", CDbl(txtTotal.Text))
                        cmd.Parameters.AddWithValue("@tgl", DateTime.Now())
                        cmd.Parameters.AddWithValue("@notran", DateTime.Now())

                        cmd.Parameters.AddWithValue("@kd", kd)
                        cmd.ExecuteNonQuery()
                        'updateStok()
                        kondisiAwal()
                        txtDisabled()
                        btnTambah.Text = "Tambah"
                        kosongkan()
                        rumusTotal()
                    End Using
                End If
                Conn.Close()
            End Using
        End If
    End Sub
    Sub cmbdata()
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_menu", conn)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                cmbKode.Items.Clear()
                While sdr.Read()
                    cmbKode.Items.Add(sdr("kode_menu"))
                End While
                sdr.Close()
            End Using
            conn.Close()
        End Using
    End Sub

    Private Sub btnBayar_Click(sender As Object, e As EventArgs) Handles btnBayar.Click
        If txtBayar.Text >= CDbl(labelTotal.Text) Then
            Dim total As Decimal
            total = CDbl(txtBayar.Text) - CDbl(labelTotal.Text)
            labelKembalian.Text = FormatCurrency(total)
        Else
            MsgBox("Nominal Pembayaran Kurang")
            txtBayar.Text = ""
        End If
    End Sub

    Private Sub txtBayar_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtBayar.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Enter) Then
            txtBayar.Text = FormatCurrency(txtBayar.Text)
        End If
    End Sub

    Private Sub btnTransaksi_Click(sender As Object, e As EventArgs) Handles btnTransaksi.Click
        Using Conn As New SqlConnection(stringConnection)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_laporan (id_transaksi,nama_menu,harga_satuan,qty,total_bayar,tgl_transaksi) values (@id,@nama,@harga,@qty,@subtotal,@tgl)", Conn)
                For Each baris As DataGridViewRow In DataGridView1.Rows
                    If Not baris.IsNewRow Then
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(baris.Cells(0).Value))
                        cmd.Parameters.AddWithValue("@nama", Convert.ToString(baris.Cells(2).Value))
                        cmd.Parameters.AddWithValue("@harga", baris.Cells(3).Value)
                        cmd.Parameters.AddWithValue("@qty", Convert.ToInt32(baris.Cells(4).Value))
                        cmd.Parameters.AddWithValue("@subtotal", baris.Cells(5).Value)
                        cmd.Parameters.AddWithValue("@tgl", Convert.ToDateTime(baris.Cells(6).Value))
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                        labelTotal.ResetText()
                        labelKembalian.ResetText()
                        txtBayar.Clear()
                    End If
                Next
            End Using
            Using cmd As New SqlCommand("delete from Tbl_transaksi", Conn)
                cmd.ExecuteNonQuery()
                kondisiAwal()
            End Using
            Conn.Close()
        End Using
        btnTambah.Text = "Tambah"
    End Sub
    Sub changelongpaper()
        Dim rowcount As Integer
        longpaper = 0
        rowcount = DataGridView1.Rows.Count
        longpaper = rowcount * 15
        longpaper = longpaper + 240
    End Sub
    Private Sub btnCetak_Click(sender As Object, e As EventArgs) Handles btnCetak.Click
        changelongpaper()
        PPD.Document = PD
        PPD.ShowDialog()
    End Sub

    Private Sub PD_BeginPrint(sender As Object, e As PrintEventArgs) Handles PD.BeginPrint
        'Dim pagesetup As New PageSettings
        'pagesetup.PaperSize = New PaperSize("Custom", 300, 500) 'fixed size
        Dim paperSize As New PaperSize("Custom", 350, 500)
        'pagesetup.PaperSize = New PaperSize("Custom", 250, longpaper)
        PD.DefaultPageSettings.PaperSize = paperSize
    End Sub

    Private Sub PD_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PD.PrintPage
        Dim f10 As New Font("Times New Roman", 10, FontStyle.Regular)
        Dim f10b As New Font("Times New Roman", 10, FontStyle.Bold)
        Dim f14 As New Font("Times New Roman", 14, FontStyle.Bold)

        Dim leftmargin As Integer = PD.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garis As String
        garis = "------------------------------------------------------------------"

        e.Graphics.DrawString("Angkringan", f14, Brushes.Black, centermargin, 5, tengah)
        e.Graphics.DrawString("JL.Raya, Cibitung, Bekasi," & vbNewLine & " Kab Bekasi, Jawa Barat", f10, Brushes.Black, centermargin, 30, tengah)
        e.Graphics.DrawString("Hp: 0856-9346-2461", f10, Brushes.Black, centermargin, 65, tengah)

        e.Graphics.DrawString("Nama Kasir :", f10, Brushes.Black, 0, 90)
        e.Graphics.DrawString("goro", f10, Brushes.Black, 75, 90)

        e.Graphics.DrawString(Date.Now(), f10, Brushes.Black, 0, 105)
        e.Graphics.DrawString("Nama", f10, Brushes.Black, 0, 125)
        e.Graphics.DrawString("qty", f10, Brushes.Black, 80, 125)
        e.Graphics.DrawString("harga", f10, Brushes.Black, 120, 125)
        e.Graphics.DrawString("Total", f10, Brushes.Black, rightmargin, 125, kanan)
        e.Graphics.DrawString(garis, f10, Brushes.Black, 0, 130)
        Dim tinggi As Integer
        Dim total As Integer
        For Each baris As DataGridViewRow In DataGridView1.Rows
            If Not baris.IsNewRow Then
                tinggi += 15
                e.Graphics.DrawString(baris.Cells(2).Value, f10, Brushes.Black, 0, 130 + tinggi)
                e.Graphics.DrawString(baris.Cells(4).Value, f10, Brushes.Black, 80, 130 + tinggi)
                e.Graphics.DrawString(baris.Cells(3).Value, f10, Brushes.Black, 120, 130 + tinggi)

                e.Graphics.DrawString(baris.Cells(5).Value, f10, Brushes.Black, rightmargin, 130 + tinggi, kanan)
                total += CDbl(baris.Cells(5).Value)
            End If
        Next
        tinggi = 140 + tinggi
        e.Graphics.DrawString(garis, f10, Brushes.Black, 0, tinggi)
        e.Graphics.DrawString("Subtotal :" & FormatCurrency(total), f10b, Brushes.Black, 150, 15 + tinggi)
    End Sub

    Private Sub Form2_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        cmbdata()
        Using Conn As New SqlConnection(stringConnection)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_laporan (id_transaksi,nama_menu,harga_satuan,qty,total_bayar,tgl_transaksi) values (@id,@nama,@harga,@qty,@subtotal,@tgl)", Conn)
                For Each baris As DataGridViewRow In DataGridView1.Rows
                    If Not baris.IsNewRow Then
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(baris.Cells(0).Value))
                        cmd.Parameters.AddWithValue("@nama", Convert.ToString(baris.Cells(2).Value))
                        cmd.Parameters.AddWithValue("@harga", baris.Cells(3).Value)
                        cmd.Parameters.AddWithValue("@qty", Convert.ToInt32(baris.Cells(4).Value))
                        cmd.Parameters.AddWithValue("@subtotal", baris.Cells(5).Value)
                        cmd.Parameters.AddWithValue("@tgl", Convert.ToDateTime(baris.Cells(6).Value))
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                        labelTotal.ResetText()
                        labelKembalian.ResetText()
                        txtBayar.Clear()
                    End If
                Next
            End Using
            Using cmd As New SqlCommand("delete from Tbl_transaksi", Conn)
                cmd.ExecuteNonQuery()
                kondisiAwal()
                txtBayar.Text = ""
                labelKembalian.Text = ""
            End Using
            Conn.Close()
        End Using
    End Sub

    Private Sub LaporanPenjualanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LaporanPenjualanToolStripMenuItem.Click
        Me.Visible = False
        Form4.Visible = True
    End Sub

    Private Sub TransaksiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransaksiToolStripMenuItem.Click
        Me.Visible = False
        Form3.Visible = True
    End Sub
End Class