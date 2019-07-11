﻿using System;
using System.Collections.Generic;
using EDIdataClass;
using System.Data.SqlClient;
using log4net;

/// <summary>
/// Summary description for Basecorpallo_corp
/// </summary>
public class Basecorpallo_corp
{
	public Basecorpallo_corp()
	{	}
    public Response insert(List<basecorpallo_corp> data, String edi)
    {
        Response response = new Response();
        SqlTransaction transaction = null;
        string sql = "Insert into basecorpallo_corp(bcode, paydesc,month_eli, year_eli, " +
                                "date_time, sys_creator,rowguid) " +
                                "values " +
                                "(@bcode,@paydesc, @month_eli, @year_eli, " +
                                "@date_time, @sys_creator) ";
        try
        {
            using (SqlConnection connection = new DBConnection().getEDIConnection(edi))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@bcode", System.Data.SqlDbType.VarChar);
                    command.Parameters.Add("@paydesc", System.Data.SqlDbType.VarChar);
                    command.Parameters.Add("@month_eli", System.Data.SqlDbType.VarChar);
                    command.Parameters.Add("@year_eli", System.Data.SqlDbType.VarChar);
                    command.Parameters.Add("@date_time", System.Data.SqlDbType.VarChar);
                    command.Parameters.Add("@sys_creator", System.Data.SqlDbType.VarChar);
                    //command.Parameters.Add("@rowguid", System.Data.SqlDbType.UniqueIdentifier);
                    command.CommandTimeout = 0;
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    try
                    {
                        int i = 0;
                        for (i = 0; i < data.Count; i++)
                        {
                            var temp = data[i];


                            command.Parameters["@bcode"].Value = temp.bcode;
                            command.Parameters["@paydesc"].Value = temp.paydesc;
                            command.Parameters["@month_eli"].Value = temp.month_eli;
                            command.Parameters["@year_eli"].Value = temp.year_eli;
                            command.Parameters["@date_time"].Value = temp.date_time;
                            command.Parameters["@sys_creator"].Value = temp.sys_creator;
                      //      command.Parameters["@rowguid"].Value = temp.rowguid;
                            command.ExecuteNonQuery();

                        }
                        if (i == data.Count)
                        {
                            transaction.Commit();
                            response.responseCode = ResponseCode.OK;
                            response.responseMessage = i.ToString() + " transactions inserted";
                        }
                        else
                        {
                            transaction.Rollback();
                            response.responseCode = ResponseCode.Error;
                            response.responseMessage = "Something went wrong ";
                        }

                        connection.Close();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        connection.Close();
                        Service.log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                        Service.log.Error(edi + " " + e.Message, e);
                        return new Response(ResponseCode.Error, e.Message);
                    }

                }
            }
        }
        catch (Exception e)
        {
            Service.log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Service.log.Error(edi + " " + e.Message, e);
            return new Response(ResponseCode.Error, e.Message);
        }


        return response;
    }
}