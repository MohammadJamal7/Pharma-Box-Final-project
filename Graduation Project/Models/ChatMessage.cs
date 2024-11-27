public class ChatMessage
{
    public int Id { get; set; }
    public string SenderId { get; set; }  // UserId of the sender (Patient or Pharmacist)
    public ApplicationUser Sender { get; set; }
    public string ReceiverId { get; set; }  // Pharmacist for patients, Patient for pharmacists
    public ApplicationUser Receiver { get; set; }
    public string MessageContent { get; set; }
    public DateTime SentDate { get; set; }
    public bool IsRead { get; set; }  // To mark whether the message has been read
}
