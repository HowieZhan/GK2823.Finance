using MailKit.Net.Smtp;
using System.Collections.Generic;
using System.Text;
using MimeKit;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using MimeKit.Text;

namespace GK2823.UtilLib.Helpers
{
    /// <summary>
    /// 基于MailKit的邮件帮助类
    /// </summary>
    public static class MailHelper
    {
        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="mailInfo">邮件对象封装</param>
        /// <param name="attachments">附件</param>
        /// <param name="dispose">是否自动释放附件所用Stream</param>
        /// <returns></returns>
        public static async Task SendAsync(MailInfo mailInfo, IEnumerable<AttachmentInfo> attachments = null, bool dispose = true)
        {
            var message = new MimeMessage();
            message.From.AddRange(mailInfo.FromAddress);
            message.To.AddRange(mailInfo.ToAddress);
            if (mailInfo.CcAddress != null)
            {
                message.Cc.AddRange(mailInfo.CcAddress);
            }
            if (mailInfo.BccAddress != null)
            {
                message.Bcc.AddRange(mailInfo.BccAddress);
            }
            message.Subject = mailInfo.Subject;
            var body = new TextPart(mailInfo.Format)
            {
                Text = mailInfo.Content
            };
            MimeEntity entity = body;
            if (attachments != null)
            {
                var mult = new Multipart("mixed")
                {
                    body
                };
                foreach (var att in attachments)
                {
                    if (att.Stream != null)
                    {
                        var attachment = string.IsNullOrWhiteSpace(att.ContentType) ? new MimePart() : new MimePart(att.ContentType);
                        attachment.Content = new MimeContent(att.Stream);
                        attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                        attachment.ContentTransferEncoding = att.ContentTransferEncoding;
                        attachment.FileName = ConvertHeaderToBase64(att.FileName, Encoding.UTF8);//解决附件中文名问题
                        mult.Add(attachment);
                    }
                }
                entity = mult;
            }
            message.Body = entity;
            message.Date = DateTime.Now;
            using (var client = new SmtpClient())
            {
                //创建连接
                await client.ConnectAsync(mailInfo.Host, mailInfo.Port, mailInfo.UseSsl).ConfigureAwait(false);
                await client.AuthenticateAsync(mailInfo.UserAddress, mailInfo.Password).ConfigureAwait(false);
                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
                if (dispose && attachments != null)
                {
                    foreach (var att in attachments)
                    {
                        att.Dispose();
                    }
                }
            }
        }
        private static string ConvertToBase64(string inputStr, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(inputStr));
        }
        private static string ConvertHeaderToBase64(string inputStr, Encoding encoding)
        {
            var encode = !string.IsNullOrEmpty(inputStr) && inputStr.Any(c => c > 127);
            if (encode)
            {
                return "=?" + encoding.WebName + "?B?" + ConvertToBase64(inputStr, encoding) + "?=";
            }
            return inputStr;
        }
    }

    public class MailInfo
    {
        /// <summary>
        /// 邮件服务器Host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 邮件服务器Port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 邮件服务器是否是ssl
        /// </summary>
        public bool UseSsl { get; set; }
        /// <summary>
        /// 发送邮件的账号友善名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 发送邮件的账号地址
        /// </summary>
        public string UserAddress { get; set; }
        /// <summary>
        /// 发现邮件所需的账号密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 邮件内容格式
        /// </summary>
        public TextFormat Format { get; set; }
        /// <summary>
        /// 发件地址
        /// </summary>
        public List<MailboxAddress> FromAddress { get; set; }
        /// <summary>
        /// 邮件地址
        /// </summary>
        public List<MailboxAddress> ToAddress { get; set; }
        /// <summary>
        /// 抄送地址
        /// </summary>
        public List<MailboxAddress> CcAddress { get; set; }
        /// <summary>
        /// 暗送地址
        /// </summary>
        public List<MailboxAddress> BccAddress { get; set; }
        public MailInfo()
        {
            this.Format = TextFormat.Html;//默认html
        }

        public void Reset()
        {
            this.FromAddress = new List<MailboxAddress>();
            this.ToAddress = new List<MailboxAddress>();
            this.CcAddress = new List<MailboxAddress>();
            this.BccAddress = new List<MailboxAddress>();

            if (!string.IsNullOrEmpty(UserAddress))
            {
                this.FromAddress.Add(new MailboxAddress(UserName, UserAddress));
            }
        }

        public void AddToAddress(string address, string name = null)
        {
            this.ToAddress.Add(new MailboxAddress(name, address));
        }

        public void AddCcAddress(string address, string name = null)
        {
            this.CcAddress.Add(new MailboxAddress(name, address));
        }

        public void AddBccAddress(string address, string name = null)
        {
            this.BccAddress.Add(new MailboxAddress(name, address));
        }
    }

    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachmentInfo : IDisposable
    {
        /// <summary>
        /// 附件类型，比如application/pdf
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件传输编码方式，默认ContentEncoding.Default
        /// </summary>
        public ContentEncoding ContentTransferEncoding { get; set; } = ContentEncoding.Default;
        /// <summary>
        /// 文件数组
        /// </summary>
        public byte[] Data { get; set; }
        private Stream stream;
        /// <summary>
        /// 文件数据流，获取数据时优先采用此部分
        /// </summary>
        public Stream Stream
        {
            get
            {
                if (this.stream == null && this.Data != null)
                {
                    stream = new MemoryStream(this.Data);
                }
                return this.stream;
            }
            set { this.stream = value; }
        }
        /// <summary>
        /// 释放Stream
        /// </summary>
        public void Dispose()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
            }
        }
    }
}
