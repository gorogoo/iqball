Imports System.Data.SqlClient
Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtUsername.Focus()
        txtPassword.PasswordChar = "*"
    End Sub
    Sub kosongkan()
        txtPassword.Text = ""
        txtUsername.Text = ""
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_user where username=@username and password=@password", conn)
                cmd.Parameters.AddWithValue("@username", txtUsername.Text)
                cmd.Parameters.AddWithValue("@password", txtPassword.Text)
                Dim rd As SqlDataReader = cmd.ExecuteReader
                If rd.HasRows Then
                    Form3.Visible = True
                    Me.Visible = False
                    kosongkan()
                Else
                    MsgBox("Password atau username salah!")
                    kosongkan()
                End If
            End Using
            conn.Close()
        End Using
    End Sub

    Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs) Handles txtPassword.TextChanged

    End Sub
End Class
