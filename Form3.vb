Imports System.Data.SqlClient
Public Class Form3
    Sub kondisiAwal()
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using sda As New SqlDataAdapter("select * from Tbl_menu order by id_menu desc ", conn)
                Using ds As New DataSet
                    ds.Clear()
                    sda.Fill(ds, "Tbl_menu")
                    DataGridView1.DataSource = ds.Tables("Tbl_menu")
                End Using
            End Using
            conn.Close()
        End Using
    End Sub
    Sub kosongkan()
        txtKode.Text = ""
        txtNama.Text = ""
        cmbSatuan.Text = ""
        txtHarga.Text = ""
    End Sub
    Sub txtenabled()
        txtHarga.Enabled = True
        txtKode.Enabled = True
        txtNama.Enabled = True
        cmbSatuan.Enabled = True
    End Sub
    Sub txtdisabled()
        txtHarga.Enabled = False
        txtKode.Enabled = False
        txtNama.Enabled = False
        cmbSatuan.Enabled = False
    End Sub
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kondisiAwal()
        txtdisabled()
        cmbSatuan.Items.Add("Pcs")
        cmbSatuan.Items.Add("Botol")
    End Sub

    Private Sub TransaksiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransaksiToolStripMenuItem.Click
        Form2.Visible = True
        Me.Visible = False
    End Sub

    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        If btnTambah.Text = "Tambah" Then
            btnTambah.Text = "Simpan"
            txtenabled()
            kosongkan()
        Else
            If txtHarga.Text = "" Or txtKode.Text = "" Or txtNama.Text = "" Or cmbSatuan.Text = "" Then
                MsgBox("Semua Field Harus Terisi")
            Else
                If MessageBox.Show("Yakin Tambah Data Menu?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                    Using conn As New SqlConnection(stringConnection)
                        conn.Open()
                        Using cmd As New SqlCommand("insert into Tbl_menu(nama_menu,kode_menu,harga_satuan,satuan) values (@nama,@kode,@harga,@satuan)", conn)
                            cmd.Parameters.AddWithValue("@kode", txtKode.Text)
                            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
                            cmd.Parameters.AddWithValue("@harga", txtHarga.Text)
                            cmd.Parameters.AddWithValue("@satuan", cmbSatuan.Text)
                            cmd.ExecuteNonQuery()
                            kondisiAwal()
                            kosongkan()
                            txtdisabled()
                            btnTambah.Text = "Tambah"
                        End Using
                        conn.Close()
                    End Using
                End If
            End If
        End If
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If btnEdit.Text = "Edit" Then
            btnEdit.Text = "Simpan"
            txtenabled()
        Else
            If txtHarga.Text = "" Or txtKode.Text = "" Or txtNama.Text = "" Or cmbSatuan.Text = "" Then
                MsgBox("Semua Field Harus Terisi")
            Else
                If MessageBox.Show("Yakin Update Data Menu?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                    Using conn As New SqlConnection(stringConnection)
                        conn.Open()
                        Using cmd As New SqlCommand("update Tbl_menu set nama_menu=@nama,satuan=@satuan,harga_satuan=@harga,kode_menu=@kode where id_menu=@id", conn)
                            cmd.Parameters.AddWithValue("@id", txtid.Text)
                            cmd.Parameters.AddWithValue("@kode", txtKode.Text)
                            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
                            cmd.Parameters.AddWithValue("@harga", txtHarga.Text)
                            cmd.Parameters.AddWithValue("@satuan", cmbSatuan.Text)
                            cmd.ExecuteNonQuery()
                            kondisiAwal()
                            kosongkan()
                            txtdisabled()
                            btnEdit.Text = "Edit"
                        End Using
                        conn.Close()
                    End Using
                End If
            End If
        End If
    End Sub

    Private Sub btnHapus_Click(sender As Object, e As EventArgs) Handles btnHapus.Click
        If txtHarga.Text = "" Or txtKode.Text = "" Or txtNama.Text = "" Or cmbSatuan.Text = "" Then
            MsgBox("Field Masih Kosong")
        Else
            If MessageBox.Show("Yakin Update Data Menu?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                Using conn As New SqlConnection(stringConnection)
                    conn.Open()
                    Using cmd As New SqlCommand("delete from Tbl_menu where id_menu=@id", conn)
                        cmd.Parameters.AddWithValue("@id", txtid.Text)
                        cmd.ExecuteNonQuery()
                        kondisiAwal()
                        kosongkan()
                    End Using
                    conn.Close()
                End Using
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            txtid.Text = IIf(IsDBNull(DataGridView1.CurrentRow.Cells(0).Value), "", DataGridView1.CurrentRow.Cells(0).Value)
            txtNama.Text = IIf(IsDBNull(DataGridView1.CurrentRow.Cells(1).Value), "", DataGridView1.CurrentRow.Cells(1).Value)
            cmbSatuan.Text = IIf(IsDBNull(DataGridView1.CurrentRow.Cells(2).Value), "", DataGridView1.CurrentRow.Cells(2).Value)
            txtHarga.Text = IIf(IsDBNull(DataGridView1.CurrentRow.Cells(3).Value), "", DataGridView1.CurrentRow.Cells(3).Value)
            txtKode.Text = IIf(IsDBNull(DataGridView1.CurrentRow.Cells(4).Value), "", DataGridView1.CurrentRow.Cells(4).Value)
        End If

    End Sub

    Private Sub txtCari_TextChanged(sender As Object, e As EventArgs) Handles txtCari.TextChanged
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_menu where nama_menu Like Concat('%',@cari,'%')", conn)
                cmd.Parameters.AddWithValue("@cari", txtCari.Text)
                Dim sda As New SqlDataAdapter(cmd)
                Using ds As New DataSet
                    sda.Fill(ds, "Tbl_menu")
                    DataGridView1.DataSource = ds.Tables("Tbl_menu")
                End Using
            End Using
            conn.Close()
        End Using
    End Sub

    Private Sub LaporanPenjualanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LaporanPenjualanToolStripMenuItem.Click
        Me.Visible = False
        Form4.Visible = True
    End Sub

    Private Sub LogoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogoutToolStripMenuItem.Click
        Me.Visible = False
        Form1.Visible = True
    End Sub

    Private Sub KelolaMenuToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KelolaMenuToolStripMenuItem.Click
        Me.Visible = False
        Form2.Visible = True
    End Sub
End Class