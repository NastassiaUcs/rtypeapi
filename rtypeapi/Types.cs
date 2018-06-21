using Newtonsoft.Json;
using System;
namespace rtypeapi
{
    public class Update
    {
        public bool ok;
        public Result[] result;
    }

    public class Result
    {
        public long update_id;
        public Message message;
    }

    public class Message
    {
        public int message_id;
        public User from;
        public Chat chat;
        public string date;
        public string edit_date;
        public string text;
        public Entities[] entities;
        public New_Chat_Participant new_chat_participant;
        public New_Chat_Member new_chat_member;
        public New_Chat_Member[] new_chat_members;
        public Message reply_to_message;
        public Sticker sticker;
    }

    public class User
    {
        public long id;
        public bool is_bot;
        public string first_name;
        public string last_name;
        public string username;
        public string language_code;

        public string GetFullName
        {
            get
            {
                string name = String.IsNullOrEmpty(last_name) ? first_name : first_name + " " + last_name;
                return name;
            }
        }

        public string NameWithUserName
        {
            get
            {
                return GetFullName + (" " + username != null && username.Length > 0 ? " @" + username : "");
            }
        }
    }

    public class Chat : User
    {
        //public string type;
        [JsonProperty(Required = Required.Always)]
        public ChatType Type { get; set; }

        public string title;
    }

    public class Entities
    {
        public int offset;
        public int length;
        [JsonProperty(Required = Required.Always)]
        public MessageEntityType Type { get; set; }
    }

    public class New_Chat_Participant : User
    {

    }

    public class New_Chat_Member : User
    {

    }

    public class Sticker
    {
        public int width;
        public int height;
        public string emoji;
        public string set_name;
        public Thumb thumb;
        public string file_id;
        public int file_size;
    }

    public class Thumb
    {
        public string file_id;
        public int file_size;
        public int width;
        public int height;
    }
}