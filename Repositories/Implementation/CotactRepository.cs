using System.Data;
using ContactProject.Models;
using ContactProject.Repositories.Interface;
using Npgsql;


namespace ContactProject.Repositories.Implementation;


public class CotactRepository : IContactInterface
{

    private readonly NpgsqlConnection _conn;

    public CotactRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> Add(t_Contact data)
    {
        try
        {
            NpgsqlCommand cm = new NpgsqlCommand(@"INSERT INTO t_contact
            (c_userid,c_contactname,c_email,c_mobile,c_address,c_image,c_status,c_group)
            VALUES (@c_userid,@c_contactname,@c_email,@c_mobile,@c_address,@c_image,@c_status,@c_group)", _conn);
            cm.Parameters.AddWithValue("@c_userid", data.c_UserId);
            cm.Parameters.AddWithValue("@c_contactname", data.c_ContactName);
            cm.Parameters.AddWithValue("@c_email", data.c_Email);
            cm.Parameters.AddWithValue("@c_mobile", data.c_Mobile);
            cm.Parameters.AddWithValue("@c_address", data.c_Address);
            cm.Parameters.AddWithValue("@c_image", data.c_Image == null ? DBNull.Value : data.c_Image);
            cm.Parameters.AddWithValue("@c_status", data.c_Status);
            cm.Parameters.AddWithValue("@c_group", data.c_Group);
            _conn.Close();
            _conn.Open();
            cm.ExecuteNonQuery();
            _conn.Close();
            return 1;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public List<t_Contact> Create()
    {
        throw new NotImplementedException();
    }


    public async Task<int> Delete(string contactid)
    {
        try
        {
            NpgsqlCommand cm = new NpgsqlCommand(@"DELETE FROM t_contact
                                                    WHERE c_contactid = @c_contactid", _conn);
            cm.Parameters.AddWithValue("@c_contactid", int.Parse(contactid));
            _conn.Close();
            _conn.Open();
            cm.ExecuteNonQuery();
            _conn.Close();
            return 1;

        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public void Delete(t_Contact data)
    {
        throw new NotImplementedException();
    }


    public async Task<List<t_Contact>> GetAll()
    {
        DataTable dt = new DataTable();
        NpgsqlCommand cm = new NpgsqlCommand("select * from t_contact", _conn);
        _conn.Close();
        _conn.Open();
        NpgsqlDataReader datar = cm.ExecuteReader();
        if (datar.HasRows)
        {
            dt.Load(datar);

        }
        List<t_Contact> contactList = new List<t_Contact>();
        contactList = (from DataRow dr in dt.Rows
                       select new t_Contact()
                       {
                           c_ContactId = Convert.ToInt32(dr["c_contactid"]),
                           c_UserId = int.Parse(dr["c_userid"].ToString()),
                           c_ContactName = dr["c_contactname"].ToString(),
                           c_Email = dr["c_email"].ToString(),
                           c_Address = dr["c_address"].ToString(),
                           c_Mobile = dr["c_mobile"].ToString(),
                           c_Image = dr["c_image"].ToString(),
                           c_Group = dr["c_group"].ToString(),
                           c_Status = dr["c_status"].ToString()

                       }).ToList();
        _conn.Close();
        return contactList;
    }

    public async Task<List<t_Contact>> GetAllByUser(string userid)
    {
        DataTable dt = new DataTable();
        NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM t_contact", _conn);
        _conn.Close();
        _conn.Open();
        NpgsqlDataReader datar = cmd.ExecuteReader();
        if (datar.HasRows)
        {
            dt.Load(datar);
        }
        List<t_Contact> contactList = new List<t_Contact>();
        contactList = (from DataRow dr in dt.Rows
                       where dr["c_userid"].ToString() == userid
                       select new t_Contact()
                       {
                           c_ContactId = Convert.ToInt32(dr["c_contactid"]),
                           c_UserId = int.Parse(dr["c_userid"].ToString()),
                           c_ContactName = dr["c_contactname"].ToString(),
                           c_Email = dr["c_email"].ToString(),
                           c_Mobile = dr["c_mobile"].ToString(),
                           c_Address = dr["c_address"].ToString(),
                           c_Image = dr["c_image"].ToString(),
                           c_Group = dr["c_group"].ToString(),
                           c_Status = dr["c_status"].ToString()
                       }).ToList();
        _conn.Close();
        return contactList;
    }

    public async Task<t_Contact> GetOne(string contactid)
    {
        DataTable dt = new DataTable();
        NpgsqlCommand cm = new NpgsqlCommand("SELECT * FROM t_contact WHERE c_contactid = @c_contactid", _conn);
        cm.Parameters.AddWithValue("@c_contactid", int.Parse(contactid));
        _conn.Close();
        _conn.Open();
        NpgsqlDataReader dr = cm.ExecuteReader();
        t_Contact contact = null;
        if (dr.Read())
        {
            Console.WriteLine("@@@@@@@@" + dr["c_email"]);
            contact = new t_Contact()
            {
                c_ContactId = Convert.ToInt32(dr["c_contactid"]),
                c_UserId = int.Parse(dr["c_userid"].ToString()),
                c_ContactName = dr["c_contactname"].ToString(),
                c_Email = dr["c_email"].ToString(),
                c_Mobile = dr["c_mobile"].ToString(),
                c_Address = dr["c_address"].ToString(),
                c_Image = dr["c_image"].ToString(),
                c_Group = dr["c_group"].ToString(),
                c_Status = dr["c_status"].ToString()
            };
        }
        _conn.Close();
        return contact;
    }

    public t_Contact GetOne(int id)
    {
        throw new NotImplementedException();
    }

    public void Insert(t_Contact data)
    {
        throw new NotImplementedException();
    }


    public async Task<int> Update(t_Contact contactData)
    {

        try
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(@"UPDATE t_contact SET
                                                    c_userid = @c_userid,
                                                    c_contactname = @c_contactname,
                                                    c_email = @c_email,
                                                    c_mobile = @c_mobile,
                                                    c_address = @c_address,
                                                    c_image = @c_image,
                                                    c_status = @c_status,
                                                    c_group = @c_group
                                                    WHERE c_contactid = @c_contactid", _conn))
            {
                cmd.Parameters.AddWithValue("@c_userid", contactData.c_UserId);
                cmd.Parameters.AddWithValue("@c_contactname", contactData.c_ContactName);
                cmd.Parameters.AddWithValue("@c_email", contactData.c_Email);
                cmd.Parameters.AddWithValue("@c_mobile", contactData.c_Mobile);
                cmd.Parameters.AddWithValue("@c_address", contactData.c_Address);
                cmd.Parameters.AddWithValue("@c_image", contactData.c_Image ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@c_status", contactData.c_Status);
                cmd.Parameters.AddWithValue("@c_group", contactData.c_Group);
                cmd.Parameters.AddWithValue("@c_contactid", contactData.c_ContactId);  // 🔹 Missing Parameter Added!

                _conn.Open();  // 🔹 Open connection before executing command
                // int rowsAffected = 
                cmd.ExecuteNonQuery();
                _conn.Close();
                return 1;
                //return rowsAffected > 0 ? 1 : 0;  // 🔹 Return 1 if update is successful, 0 otherwise
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating contact: " + ex.Message);  // 🔹 Log error message for debugging
            return 0;
        }
    }

}

