using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CCWin;
using System.Data.SqlClient;
using System.Collections;
using ObjectiveSQL;

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
        ArrayList allDB;
        Array TBArray;


        bool updateStatus = false;
        bool addStatus = false;

        string con = "Data Source=.;Initial Catalog={0};Integrated Security=True", sql;

        public Form1()
        {
            InitializeComponent();
            skinGroupBox3.VerticalScroll.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            allDB = GetAllDataBase(".");
            DBComboBox.DataSource = allDB;
            skinButton2.Enabled = false;
            skinButton3.Enabled = false;
        }

        // 获取全部数据库
        private ArrayList GetAllDataBase(string ip)
        {
            ArrayList DBNameList = new ArrayList();
            SqlConnection Connection = new SqlConnection(
                String.Format("Data Source={0};Initial Catalog = master;Integrated Security=True",ip));
            DataTable DBNameTable = new DataTable();
            SqlDataAdapter Adapter = new SqlDataAdapter("select name from master..sysdatabases", Connection);

            lock (Adapter)
            {
                Adapter.Fill(DBNameTable);
            }

            foreach (DataRow row in DBNameTable.Rows)
            {
                DBNameList.Add(row["name"]);
            }

            return DBNameList;
        }

        // 选择数据库后,获取全部表并显示
        private void DBComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myCon != null)
            {
                if (myCon.State == ConnectionState.Open)
                    myCon.Close();
            }
            con = string.Format("Data Source=.;Initial Catalog={0};Integrated Security=True", DBComboBox.SelectedItem.ToString());
            myCon = new SqlConnection(con);
            try
            {
                myCon.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            SqlCommand cmdToTables = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", myCon);
            SqlDataReader dr = cmdToTables.ExecuteReader();
            ArrayList TBList = new ArrayList();

            while (dr.Read())
            {
                TBList.Add(dr.GetString(0));
            }
            TBArray = TBList.ToArray();
            TBComboBox.DataSource = TBList;
            if (TBComboBox.Items.Count == 0)
                TBComboBox.Text = "";

            dr.Close();
            skinButton3.Enabled = false;
            skinButton2.Enabled = false;
        }

        private void TBComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            skinButton3.Enabled = false;
            skinButton2.Enabled = false;
        }

        // 连接数据库, 刷新条件查询控件
        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (TBComboBox.SelectedItem != null)
                sql = string.Format("SELECT * from {0}", TBComboBox.SelectedItem.ToString());
            else
            {
                MessageBox.Show("没有表，连接失败，呵呵哒！");
                return;
            }

            skinGroupBox3.Controls.Clear();

            if (myCon.State == ConnectionState.Open)
            {
                myda = new SqlDataAdapter(sql, con);
                DataSet myds = new DataSet();

                //   SqlCommand cmdToTables = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", myCon);
                //    SqlDataReader dr = cmdToTables.ExecuteReader();
                //    ArrayList TBList = new ArrayList();

                //    while (dr.Read())
                //    {
                //        TBList.Add(dr.GetString(0));
                //    }
                //    Array TBArray = TBList.ToArray();
                //    TBComboBox.DataSource = TBList;

                myda.Fill(myds, TBArray.GetValue(0).ToString());
                skinDataGridView1.DataSource = myds.Tables[TBArray.GetValue(0).ToString()];


                closeCommand = myCon.CreateCommand();
                closeCommand.CommandType = CommandType.Text;

                orids = new DataSet();
                myda.Fill(orids, TBArray.GetValue(0).ToString());
                for (int i = 0; i < orids.Tables[TBArray.GetValue(0).ToString()].Rows.Count; i++)
                {
                    oriList.Add(orids.Tables[TBArray.GetValue(0).ToString()].Rows[i].ItemArray[0].ToString());
                }

                int basicX = 16, basicY = 44;

                for (int col = 0; col < skinDataGridView1.Columns.Count; col++)
                {
                    //  MessageBox.Show(skinDataGridView1.Columns[col].ValueType.ToString());     // 显示每一列的type
                    int autoTitleWidth = skinDataGridView1.Columns[col].Name.Length * (int)skinDataGridView1.HeadFont.SizeInPoints;
                    if (skinDataGridView1.Columns[col].Width < autoTitleWidth)
                        skinDataGridView1.Columns[col].Width = autoTitleWidth;
                    Color c = Color.FromArgb(100, 255, 255, 255);
                    switch (skinDataGridView1.Columns[col].ValueType.ToString())
                    {
                        case "System.String":
                            #region System.String
                            CCWin.SkinControl.SkinLabel labelN = new CCWin.SkinControl.SkinLabel();
                            labelN.Name = skinDataGridView1.Columns[col].Name;
                            labelN.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            labelN.TextAlign = ContentAlignment.MiddleLeft;
                            labelN.Location = new Point(basicX, basicY * (col + 1));
                            labelN.Text = skinDataGridView1.Columns[col].Name;
                            labelN.AutoSize = true;
                            skinGroupBox3.Controls.Add(labelN);

                            CCWin.SkinControl.SkinWaterTextBox textBoxN = new CCWin.SkinControl.SkinWaterTextBox();
                            textBoxN.Name = skinDataGridView1.Columns[col].Name + "TextBoxN";
                            textBoxN.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            textBoxN.TextAlign = HorizontalAlignment.Left;
                            textBoxN.Location = new Point(labelN.Location.X + labelN.Size.Width, labelN.Location.Y);
                            textBoxN.WaterColor = c;
                            textBoxN.WaterText = skinDataGridView1.Columns[col].ValueType.ToString();
                            skinGroupBox3.Controls.Add(textBoxN);
                            #endregion
                            break;

                        case "System.Int32":
                            #region System.Int32
                            CCWin.SkinControl.SkinLabel labelINT = new CCWin.SkinControl.SkinLabel();
                            labelINT.Name = skinDataGridView1.Columns[col].Name;
                            labelINT.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            labelINT.TextAlign = ContentAlignment.MiddleLeft;
                            labelINT.Location = new Point(basicX, basicY * (col + 1));
                            labelINT.Text = skinDataGridView1.Columns[col].Name;
                            labelINT.AutoSize = true;
                            skinGroupBox3.Controls.Add(labelINT);

                            CCWin.SkinControl.SkinWaterTextBox textBoxINT1 = new CCWin.SkinControl.SkinWaterTextBox();
                            textBoxINT1.AutoSize = false;
                            textBoxINT1.Size = new System.Drawing.Size(50, labelINT.Size.Height);
                            textBoxINT1.Name = skinDataGridView1.Columns[col].Name + "TextBox1";
                            textBoxINT1.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            textBoxINT1.TextAlign = HorizontalAlignment.Left;
                            textBoxINT1.Location = new Point(labelINT.Location.X + labelINT.Size.Width, labelINT.Location.Y);
                            textBoxINT1.WaterColor = c;
                            textBoxINT1.WaterText = skinDataGridView1.Columns[col].ValueType.ToString();
                            textBoxINT1.KeyPress += new KeyPressEventHandler(textBox_Validating);
                            skinGroupBox3.Controls.Add(textBoxINT1);

                            CCWin.SkinControl.SkinLabel labelINTtag = new CCWin.SkinControl.SkinLabel();
                            labelINTtag.AutoSize = false;
                            labelINTtag.Size = new System.Drawing.Size(20, labelINT.Size.Height);
                            labelINTtag.Name = "tag";
                            labelINTtag.Font = new System.Drawing.Font("微软雅黑", 14, FontStyle.Regular);
                            labelINTtag.TextAlign = ContentAlignment.MiddleLeft;
                            labelINTtag.Location = new Point(textBoxINT1.Location.X + textBoxINT1.Size.Width, labelINT.Location.Y);
                            labelINTtag.Text = "~";
                            skinGroupBox3.Controls.Add(labelINTtag);

                            CCWin.SkinControl.SkinWaterTextBox  textBoxINT2 = new CCWin.SkinControl.SkinWaterTextBox();
                            textBoxINT2.AutoSize = false;
                            textBoxINT2.Size = new System.Drawing.Size(50, labelINT.Size.Height);
                            textBoxINT2.Name = skinDataGridView1.Columns[col].Name + "TextBox2";
                            textBoxINT2.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            textBoxINT2.TextAlign = HorizontalAlignment.Left;
                            textBoxINT2.Location = new Point(labelINTtag.Location.X + labelINTtag.Size.Width, labelINTtag.Location.Y);
                            textBoxINT2.WaterColor = c;
                            textBoxINT2.WaterText = skinDataGridView1.Columns[col].ValueType.ToString();
                            textBoxINT2.KeyPress += new KeyPressEventHandler(textBox_Validating);
                            skinGroupBox3.Controls.Add(textBoxINT2);
                            #endregion
                            break;

                        case "System.Boolean":
                            #region System.Boolean
                            CCWin.SkinControl.SkinGroupBox groupBox = new CCWin.SkinControl.SkinGroupBox();
                            groupBox.AutoSize = false;
                            groupBox.Size = new System.Drawing.Size(285, basicY);
                            groupBox.Location = new Point(basicX, basicY * (col + 1));
                            groupBox.Text = skinDataGridView1.Columns[col].Name;
                            skinGroupBox3.Controls.Add(groupBox);

                            CCWin.SkinControl.SkinRadioButton radioBtnTrue = new CCWin.SkinControl.SkinRadioButton();
                            radioBtnTrue.AutoSize = false;
                            radioBtnTrue.Name = "True";
                            radioBtnTrue.Size = new System.Drawing.Size(50, 20);
                            radioBtnTrue.Font = new System.Drawing.Font("微软雅黑", 9, FontStyle.Regular);
                            radioBtnTrue.Location = new Point(55, 20);
                            if (groupBox.Text == "sex")
                                radioBtnTrue.Text = "男";
                            else
                                radioBtnTrue.Text = "True";
                            groupBox.Controls.Add(radioBtnTrue);

                            CCWin.SkinControl.SkinRadioButton radioBtnFalse = new CCWin.SkinControl.SkinRadioButton();
                            radioBtnFalse.AutoSize = false;
                            radioBtnFalse.Name = "False";
                            radioBtnFalse.Size = new System.Drawing.Size(50, 20);
                            radioBtnFalse.Font = new System.Drawing.Font("微软雅黑", 9, FontStyle.Regular);
                            radioBtnFalse.Location = new Point(135, 20);
                            if (groupBox.Text == "sex")
                                radioBtnFalse.Text = "女";
                            else
                                radioBtnFalse.Text = "False";
                            groupBox.Controls.Add(radioBtnFalse);

                            CCWin.SkinControl.SkinRadioButton radioBtnAll = new CCWin.SkinControl.SkinRadioButton();
                            radioBtnAll.AutoSize = false;
                            radioBtnAll.Name = "All";
                            radioBtnAll.Size = new System.Drawing.Size(50, 20);
                            radioBtnAll.Font = new System.Drawing.Font("微软雅黑", 9, FontStyle.Regular);
                            radioBtnAll.Location = new Point(215, 20);
                            radioBtnAll.Text = "ALL";
                            groupBox.Controls.Add(radioBtnAll);
                            #endregion
                            break;

                        case "System.Double":
                            #region System.Double
                            CCWin.SkinControl.SkinLabel labelDouble = new CCWin.SkinControl.SkinLabel();
                            labelDouble.Name = skinDataGridView1.Columns[col].Name;
                            labelDouble.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            labelDouble.TextAlign = ContentAlignment.MiddleLeft;
                            labelDouble.Location = new Point(basicX, basicY * (col + 1));
                            labelDouble.Text = skinDataGridView1.Columns[col].Name;
                            labelDouble.AutoSize = true;
                            skinGroupBox3.Controls.Add(labelDouble);

                            CCWin.SkinControl.SkinWaterTextBox textBoxDouble1 = new CCWin.SkinControl.SkinWaterTextBox();
                            textBoxDouble1.AutoSize = false;
                            textBoxDouble1.Size = new System.Drawing.Size(50, labelDouble.Size.Height);
                            textBoxDouble1.Name = skinDataGridView1.Columns[col].Name + "TextBox1";
                            textBoxDouble1.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            textBoxDouble1.TextAlign = HorizontalAlignment.Left;
                            textBoxDouble1.Location = new Point(labelDouble.Location.X + labelDouble.Size.Width, labelDouble.Location.Y);
                            textBoxDouble1.WaterColor = c;
                            textBoxDouble1.WaterText = skinDataGridView1.Columns[col].ValueType.ToString();
                            textBoxDouble1.KeyPress += new KeyPressEventHandler(double_Validating);
                            skinGroupBox3.Controls.Add(textBoxDouble1);

                            CCWin.SkinControl.SkinLabel labelDoubletag = new CCWin.SkinControl.SkinLabel();
                            labelDoubletag.AutoSize = false;
                            labelDoubletag.Size = new System.Drawing.Size(20, labelDouble.Size.Height);
                            labelDoubletag.Name = "tag";
                            labelDoubletag.Font = new System.Drawing.Font("微软雅黑", 14, FontStyle.Regular);
                            labelDoubletag.TextAlign = ContentAlignment.MiddleLeft;
                            labelDoubletag.Location = new Point(textBoxDouble1.Location.X + textBoxDouble1.Size.Width, textBoxDouble1.Location.Y);
                            labelDoubletag.Text = "~";
                            skinGroupBox3.Controls.Add(labelDoubletag);

                            CCWin.SkinControl.SkinWaterTextBox textBoxDouble2 = new CCWin.SkinControl.SkinWaterTextBox();
                            textBoxDouble2.AutoSize = false;
                            textBoxDouble2.Size = new System.Drawing.Size(50, labelDouble.Size.Height);
                            textBoxDouble2.Name = skinDataGridView1.Columns[col].Name + "TextBox2";
                            textBoxDouble2.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            textBoxDouble2.TextAlign = HorizontalAlignment.Left;
                            textBoxDouble2.Location = new Point(labelDoubletag.Location.X + labelDoubletag.Size.Width, labelDoubletag.Location.Y);
                            textBoxDouble2.WaterColor = c;
                            textBoxDouble2.WaterText = skinDataGridView1.Columns[col].ValueType.ToString();
                            textBoxDouble2.KeyPress += new KeyPressEventHandler(double_Validating);
                            skinGroupBox3.Controls.Add(textBoxDouble2);
                            #endregion
                            break;

                        case "System.DateTime":
                            #region  System.DateTime
                            CCWin.SkinControl.SkinLabel labelTime = new CCWin.SkinControl.SkinLabel();
                            labelTime.Name = skinDataGridView1.Columns[col].Name;
                            labelTime.Font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Regular);
                            labelTime.TextAlign = ContentAlignment.MiddleLeft;
                            labelTime.Location = new Point(basicX, basicY * (col + 1));
                            labelTime.Text = skinDataGridView1.Columns[col].Name;
                            labelTime.AutoSize = true;
                            skinGroupBox3.Controls.Add(labelTime);

                            CCWin.SkinControl.SkinDateTimePicker dtPicker = new CCWin.SkinControl.SkinDateTimePicker();
                            dtPicker.Text = "";
                            dtPicker.font = new System.Drawing.Font("微软雅黑", 12, FontStyle.Bold);
                            dtPicker.Name = skinDataGridView1.Columns[col].Name;
                            skinGroupBox3.Controls.Add(dtPicker);
                            dtPicker.Location = new Point(labelTime.Location.X + labelTime.Width, labelTime.Location.Y);
                            #endregion
                            break;
                    }
                }

            }

            skinButton3.Enabled = true;
            skinButton2.Enabled = true;
        }

        void textBox_Validating(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }  
        }

        void double_Validating(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && (!Char.IsDigit(e.KeyChar) && e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }



        #region 保存（更新 & 插入）
        private void skinButton2_Click(object sender, EventArgs e)
        {
            oriList.Clear();
            updateStatus = false;
            addStatus = false;

            if (TBComboBox.SelectedItem != null)
                sql = string.Format("SELECT * from {0}", TBComboBox.SelectedItem.ToString());
            else
            {
                MessageBox.Show("木有表，保存失败，呵呵哒！");
                return;
            }
                
            myda = new SqlDataAdapter(sql, con);
            orids = new DataSet();
            myda.Fill(orids, TBComboBox.SelectedItem.ToString());

            for (int x = 0; x < orids.Tables[TBComboBox.SelectedItem.ToString()].Rows.Count; x++)
            {
                oriList.Add(orids.Tables[TBComboBox.SelectedItem.ToString()].Rows[x].ItemArray[0].ToString());
            }


            for (int a = 0; a < skinDataGridView1.Rows.Count - 1; a++)
            {
                dataGridList.Add(skinDataGridView1.Rows[a].Cells[0].Value.ToString());
            }


            for (int i = 0; i < skinDataGridView1.Rows.Count - 1; i++)
            {
                /*
                if (skinDataGridView1.Rows[i].Cells[0].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[1].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[2].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[3].Value.ToString() == "" ||
                    skinDataGridView1.Rows[i].Cells[4].Value.ToString() == "")
                {
                    MessageBox.Show("格式错误，值不能为null");
                    return;
                }
                 */
                


                for (int p = 0; p < skinDataGridView1.Rows[i].Cells.Count; p++)
                {
                    if (skinDataGridView1.Rows[i].Cells[p].Value.ToString() == "" && (skinDataGridView1.Rows[i].Cells[p].ValueType != typeof(System.DateTime)))
                    {
                        MessageBox.Show("格式错误，值不能为null");
                        return;
                    }
                }
            



                    try
                    {
                        if (oriList.Contains(skinDataGridView1.Rows[i].Cells[0].Value.ToString()))
                        {
                            /*
                            closeCommand.CommandText = string.Format("UPDATE student set name = '{0}' ,age = '{1}' ,sex = '{2}' ,height = '{3}' ,weight = '{4}' where name = '{5}'",
                                    skinDataGridView1.Rows[i].Cells[0].Value,
                                    Int16.Parse(skinDataGridView1.Rows[i].Cells[1].Value.ToString()),
                                    skinDataGridView1.Rows[i].Cells[2].Value,
                                    skinDataGridView1.Rows[i].Cells[3].Value,
                                    skinDataGridView1.Rows[i].Cells[4].Value,
                                    skinDataGridView1.Rows[i].Cells[0].Value
                            );
                             */

                            SQL.Update sqlUpdate = SQL.UPDATE(TBComboBox.SelectedItem.ToString());
                            string tempForNullDateTime = "";
                            for (int varm = 0; varm < skinDataGridView1.Rows[i].Cells.Count; varm++)
                            {
                                if (skinDataGridView1.Columns[varm].ValueType == typeof(System.DateTime))
                                    tempForNullDateTime = skinDataGridView1.Columns[varm].Name;
                                sqlUpdate.Set(skinDataGridView1.Columns[varm].Name, skinDataGridView1.Rows[i].Cells[varm].Value.ToString());
                            }
                            string where = skinDataGridView1.Columns[0].Name + " = ?";
                            sqlUpdate.Where(where, "'"+skinDataGridView1.Rows[i].Cells[0].Value.ToString()+"'");
                            Command command = sqlUpdate.toCommand();
                            closeCommand.CommandText = command.getStatement().Replace(tempForNullDateTime+" = ''",tempForNullDateTime+" = null");

                           //  MessageBox.Show(command.getStatement());

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
                            /*
                            closeCommand.CommandText = string.Format("INSERT INTO student (name, age, sex, height, weight) VALUES ('{0}','{1}','{2}','{3}','{4}')",
                                    skinDataGridView1.Rows[i].Cells[0].Value,
                                    Int16.Parse(skinDataGridView1.Rows[i].Cells[1].Value.ToString()),
                                    skinDataGridView1.Rows[i].Cells[2].Value,
                                    skinDataGridView1.Rows[i].Cells[3].Value,
                                    skinDataGridView1.Rows[i].Cells[4].Value
                            );
                            MessageBox.Show(closeCommand.CommandText);
                             */

                            SQL.Insert sqlInsert = SQL.INSERT(TBComboBox.SelectedItem.ToString());
                            for (int col = 0; col < skinDataGridView1.Columns.Count; col++)
                            {
                                sqlInsert.Values(skinDataGridView1.Columns[col].Name, "'"+skinDataGridView1.Rows[i].Cells[col].Value.ToString()+"'");
                            }
                            Command  insert = sqlInsert.toCommand();
                            closeCommand.CommandText = insert.getStatement().Replace(",''", ",null");

                            
                           // MessageBox.Show(insert.getStatement());

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
                        if (ex.Message == "未将对象引用设置到对象的实例。")
                            MessageBox.Show("保存失败，未连接到数据库！");
                        else
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
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myCon != null)
            {
                if (myCon.State == ConnectionState.Open)
                    myCon.Close();
            }
        }


        // 鼠标右键的事件
        private void skinDataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                skinContextMenuStripRow.Show(MousePosition.X, MousePosition.Y);
            }

        }

        #region 删除
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in skinDataGridView1.SelectedRows)
            {
                if (row.Selected == true)
                {
                    string where = skinDataGridView1.Columns[0].Name +" = ?";
                    SQL.Delete sqlRemove = SQL.DELETE(TBComboBox.SelectedItem.ToString()).Where(where, "'" + row.Cells[0].Value.ToString() + "'");
                    Command remove = sqlRemove.toCommand();

                    closeCommand.CommandText = remove.getStatement();
                    closeCommand.ExecuteNonQuery();
                    if (oriList.Count != 0 && orids.Tables[TBComboBox.SelectedItem.ToString()].Rows.Count != row.Index)
                    {
                        oriList.Remove(orids.Tables[TBComboBox.SelectedItem.ToString()].Rows[row.Index].ItemArray[0].ToString());
                        skinDataGridView1.Rows.RemoveAt(row.Index);
                        MessageBox.Show("删除成功！");
                    }
                    else
                        return;
                    
                }
            }
        }
        #endregion


        #region 条件查询
        private void skinButton3_Click(object sender, EventArgs e)
        {
            /*
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
             */
            SQL.Select sqlSearch;

            if (TBComboBox.SelectedItem != null)
            {
                sqlSearch = SQL.SELECT("*").From(TBComboBox.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("没有表，呵呵哒！");
                return;
            }
            for (int box = 0; box < skinGroupBox3.Controls.Count; box++)
            {
                if (skinGroupBox3.Controls[box].GetType() == typeof(CCWin.SkinControl.SkinWaterTextBox))
                {
                    CCWin.SkinControl.SkinWaterTextBox gbox = (CCWin.SkinControl.SkinWaterTextBox)skinGroupBox3.Controls[box];
                    string name = gbox.Name.Remove(gbox.Name.Length - 8);
                    
                    if (gbox.Text != "" && gbox.WaterText == "System.String")
                    {
                        string searchData = "'" + gbox.Text.ToString() + "'";
                        name = name+" = ?";
                        if (box == 0)
                        {
                            sqlSearch.Where(name, searchData);
                        }
                        else
                        {
                            sqlSearch.And(name, searchData );
                        }
                    }
                    else if (gbox.Text != "" && (gbox.WaterText == "System.Double" || gbox.WaterText == "System.Int32"))
                    {
                        string searchData = gbox.Text.ToString();
                        if (gbox.Name.EndsWith("1"))
                        {
                            name = name + " >= ?";
                            if (box == 0)
                            {
                                sqlSearch.Where(name, searchData);
                            }
                            else
                            {
                                sqlSearch.And(name, searchData);
                            }
                        }
                        else if (gbox.Name.EndsWith("2"))
                        {
                            name = name + " <= ?";
                            if (box == 0)
                            {
                                sqlSearch.Where(name, searchData);
                            }
                            else
                            {
                                sqlSearch.And(name, searchData);
                            }
                        }
                    }
                }

                if (skinGroupBox3.Controls[box].GetType() == typeof(CCWin.SkinControl.SkinGroupBox))
                {
                    CCWin.SkinControl.SkinGroupBox boolGbox = (CCWin.SkinControl.SkinGroupBox)skinGroupBox3.Controls[box];
                    string boolName = boolGbox.Text+" = ?";
                    foreach (CCWin.SkinControl.SkinRadioButton radioBtn in boolGbox.Controls)
                    {
                        string searchData = "'" + radioBtn.Name.ToString() + "'";
                        if (radioBtn.Checked == true && radioBtn.Name != "All")
                        {
                            if (box == 0)
                            {
                                sqlSearch.Where(boolName, searchData);
                            }
                            else
                            {
                                sqlSearch.And(boolName, searchData);
                            }
                        }
                        else
                        {

                        }

                    }
                }

                if (skinGroupBox3.Controls[box].GetType() == typeof(CCWin.SkinControl.SkinDateTimePicker) && skinGroupBox3.Controls[box].Text != "")
                {
                    CCWin.SkinControl.SkinDateTimePicker dtBox = (CCWin.SkinControl.SkinDateTimePicker)skinGroupBox3.Controls[box];
                    string dtName = dtBox.Name + " = ?";
                    string searchData = "'" + dtBox.Text + "'"; 
                    if (box == 0)
                    {
                        sqlSearch.Where(dtName, searchData);
                    }
                    else
                    {
                        sqlSearch.And(dtName, searchData);
                    }
                }

            }
            Command cmdSearch = sqlSearch.toCommand();
          //  MessageBox.Show(cmdSearch.getStatement()); //DEBUG

            if (myCon.State == ConnectionState.Open)
            {
                myda = new SqlDataAdapter(cmdSearch.getStatement(), con);
                DataSet myds = new DataSet();
                myda.Fill(myds, TBComboBox.SelectedItem.ToString());
                skinDataGridView1.DataSource = myds.Tables[TBComboBox.SelectedItem.ToString()];
            }

        }
        #endregion

        // boolean类型的值默认赋为false
        private void skinDataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            for (int de = 0; de < e.Row.Cells.Count; de++)
            {
                if (e.Row.Cells[de].ValueType == typeof(bool) )
                {
                    e.Row.Cells[de].Value = false;
                }
            }
        }

        

        

        

    }


}
