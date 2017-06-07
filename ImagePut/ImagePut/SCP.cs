using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSCP;

namespace ImagePut
{
    public class MessageEventArgss : EventArgs
    {
        public String Message;
    }

    public class SCP
    {
        private String hostname;
        private String username;
        private String password;
        private String fingerprint;

        public SCP(
            String hostname,
            String username,
            String password,
            String fingerprint
            )
        {
            this.hostname = hostname;
            this.username = username;
            this.password = password;
            this.fingerprint = fingerprint;
        }

        public event EventHandler<MessageEventArgss> MessageHandler;
        private void OnMessage(String message)
        {
            MessageEventArgss e = new MessageEventArgss();
            e.Message = message;
            MessageHandler(this, e);
        }

        public Boolean Put(String source, String destination)
        {
            try
            {
                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = this.hostname,
                    UserName = this.username,
                    Password = this.password,
                    SshHostKeyFingerprint = this.fingerprint
                };

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    String mkdircmd = String.Format("mkdir -p {0}", destination);
                    session.ExecuteCommand(mkdircmd);

                    //session.CreateDirectory(destination);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult = session.PutFiles(@source, destination, false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                OnMessage(String.Format("Error: {0}", e));
                return false;
            }
        }
    }


}
