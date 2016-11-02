﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CCWin;
using System.Data.SqlClient;

namespace MySQLServer
{
    public partial class Form1 : Skin_Mac
    {
        SqlConnection myCon;
        SqlDataAdapter myda;
        SqlCommand closeCommand;
        DataSet orids;
        List<string> dataGridList = new List<string>();
        List<string> oriList = new List<string>();

        bool updateStatus = false;
        bool addStatus = false;

        string con = "Data Source=.;Initial Catalog=MyExercise;Integrated Security=True", sql;

        public Form1()
        {
            InitializeComponent();
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            sql = "SELECT * from student";
            myCon = new SqlConnection(con);
            try
            {
                myCon.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (myCon.State == ConnectionState.Open)
                {
                    myda = new SqlDataAdapter(sql, con);
                    DataSet myds = new DataSet();
                    myda.Fill(myds, "student");
                    skinDataGridView1.DataSource = myds.Tables["student"];
                  
                    closeCommand = myCon.CreateCommand();
                    closeCommand.CommandType = CommandType.Text;

                    orids = new DataSet();
                    myda.Fill(orids, "student");
                    for (int i = 0; i < orids.Tables["student"].Rows.Count; i++)
                    {
                        oriList.Add(orids.Tables["student"].Rows[i].ItemArray[0].ToString());
                    }
                }
            }
        }


        private void skinButton2_Click(object sender, EventArgs e)
        {
            oriList.Clear();
            updateStatus = false;
            addStatus = false;

            sql = "SELECT * from student";
            myda = new SqlDataAdapter(sql, con);
            orids = new DataSet();
            myda.Fill(orids, "student");

            for (int x = 0; x < orids.Tables["student"].Rows.Count; x++)
            {
                oriList.Add(orids.Tables["student"].Rows[x].ItemArray[0].ToString());
            }


            for (int a = 0; a < skinDataGridView1.Rows.Count - 1; a++)
            {
                dataGridList.Add(skinDataGridView1.Rows[a].Cells[0].Value.ToString());
            }


            for (int i = 0; i < skinDataGridView1.Rows.Count - 1; i++)
            {

                if (skinDataGridView1.Rows[i].Cells[0].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[1].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[2].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[3].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[4].Value.ToString() == "")
                {
                    MessageBox.Show("格式错误，值不能为null");
                    return;
                }


                try
                {
                    if (oriList.Contains(skinDataGridView1.Rows[i].Cells[0].Value.ToString()))
                    {
                        closeCommand.CommandText = string.Format("UPDATE student set name = '{0}' ,age = '{1}' ,sex = '{2}' ,height = '{3}' ,weight = '{4}' where name = '{5}'",
                                skinDataGridView1.Rows[i].Cells[0].Value,
                                Int16.Parse(skinDataGridView1.Rows[i].Cells[1].Value.ToString()),
                                skinDataGridView1.Rows[i].Cells[2].Value,
                                skinDataGridView1.Rows[i].Cells[3].Value,
                                skinDataGridView1.Rows[i].Cells[4].Value,
                                skinDataGridView1.Rows[i].Cells[0].Value
                        );
                        try
                        {
                            closeCommand.ExecuteNonQuery();
                            updateStatus = true;
                        }
                        catch (Exception updateEx)
                        {
                            updateStatus = false;
                            MessageBox.Show(updateEx.Message);
                            return;
                        }
                    }
                    else
                    {
                        closeCommand.CommandText = string.Format("INSERT INTO student (name, age, sex, height, weight) VALUES ('{0}','{1}','{2}','{3}','{4}')",
                                skinDataGridView1.Rows[i].Cells[0].Value,
                                Int16.Parse(skinDataGridView1.Rows[i].Cells[1].Value.ToString()),
                                skinDataGridView1.Rows[i].Cells[2].Value,
                                skinDataGridView1.Rows[i].Cells[3].Value,
                                skinDataGridView1.Rows[i].Cells[4].Value
                        );
                        try
                        {
                            closeCommand.ExecuteNonQuery();
                            addStatus = true;
                        }
                        catch (Exception addEx)
                        {
                            MessageBox.Show(addEx.Message);
                            addStatus = false;
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

            }
            dataGridList.Clear();
            oriList.Clear();

            if (updateStatus == true)
                MessageBox.Show("更新成功！");
            if (addStatus == true)
                MessageBox.Show("添加成功！");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myCon != null)
            {
                if (myCon.State == ConnectionState.Open)
                    myCon.Close();
            }
        }



        private void skinDataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                skinContextMenuStripRow.Show(MousePosition.X, MousePosition.Y);
            }

        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in skinDataGridView1.SelectedRows)
            {
                if (row.Selected == true)
                {
                    closeCommand.CommandText = string.Format("DELETE FROM student WHERE name = '{0}'",
                                row.Cells[0].Value.ToString());
                    closeCommand.ExecuteNonQuery();
                    if (oriList.Count != 0 && orids.Tables["student"].Rows.Count != row.Index)
                        oriList.Remove(orids.Tables["student"].Rows[row.Index].ItemArray[0].ToString());
                    else
                        return;
                    skinDataGridView1.Rows.RemoveAt(row.Index);
                    MessageBox.Show("删除成功！");
                }
            }
        }

        private void skinButton3_Click(object sender, EventArgs e)
        {
            bool sex = false;

            sql = string.Format("SELECT * FROM student WHERE name = '{0}' and age >= {1} and age <= {2} and sex = '{3}' and height >= {4} and height <= {5} and weight >= {6} and weight <= {7}",
                nameTextBox.Text,
                ageLeastTextBox.Text,
                ageMostTextBox.Text,
                sex.ToString(),
                heightLeastTextBox.Text,
                heightMostTextBox.Text,
                weightLeastTextBox.Text,
                weightMostTextBox.Text
                );

            if (nameTextBox.Text == "")
                sql = sql.Replace(" name = '' and", "");
            if (ageLeastTextBox.Text == "")
                sql = sql.Replace(" age >=  and", "");
            if (ageMostTextBox.Text == "")
                sql = sql.Replace(" age <=  and", "");
            if (maleRadioButton.Checked)
                sql = sql.Replace("sex = 'False'", "sex = 'True'");
            if (allRadioButton.Checked)
                sql = sql.Replace(" sex = 'False' and", "");
            if (heightLeastTextBox.Text == "")
                sql = sql.Replace(" height >=  and", "");
            if (heightMostTextBox.Text == "")
                sql = sql.Replace(" height <=  and", "");
            if (weightLeastTextBox.Text == "")
                sql = sql.Replace(" weight >=  and", "");
            if (weightMostTextBox.Text == "")
                sql = sql.Replace(" weight <= ", "");
            if (sql == "SELECT * FROM student WHERE")
                sql = sql.Replace(" WHERE", "");
            if (sql.EndsWith(" and"))
                sql = sql.Remove(sql.Count() - 4);

            //MessageBox.Show(sql);

            if (myCon.State == ConnectionState.Open)
            {
                myda = new SqlDataAdapter(sql, con);
                DataSet myds = new DataSet();
                myda.Fill(myds, "student");
                skinDataGridView1.DataSource = myds.Tables["student"];
            }

        }

    }


}