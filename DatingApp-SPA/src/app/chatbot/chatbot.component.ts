import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent implements OnInit {
  messages: any[];
  loading: boolean;

  constructor() {
    this.messages = [];
    this.loading = false;
  }

  ngOnInit() {
    this.addBotMessage('Bot is here!');
  }

  handleUserMassage(event: any) {
    console.log(event);
    const text = event.message;

    this.addUserMessage(text);

    this.loading = true;

    setTimeout(() => {
      this.messages.push({
        text: 'Please integrate me with Clinc Conversational AI Platform :)',
        sender: 'Health Bot',
        avatar: '../../assets/user.png',
        date: new Date()
      });
      this.loading = false;
    }, 500);
  }

  addUserMessage(text: any) {
    this.messages.push({
      text,
      sender: 'Andrii',
      reply: true,
      date: new Date()
    });
  }

  addBotMessage(text: any) {
    this.messages.push({
      text,
      sender: 'Bot',
      avatar: '../../assets/user.png',
      date: new Date()
    });
  }
}
