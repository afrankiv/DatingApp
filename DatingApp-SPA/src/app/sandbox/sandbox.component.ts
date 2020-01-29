import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-sandbox',
  templateUrl: './sandbox.component.html',
  styleUrls: ['./sandbox.component.css']
})
export class SandboxComponent implements OnInit {

  constructor() { }

  ngOnInit() {
    // debugger;
    const greetingPoster = new Promise((resolve, reject) => {
      console.log('PROMISE 1: Inside Promise (proof of being eager)');
      resolve('Welcome! Nice to meet you.');
    });

    console.log('PROMISE 2: Before calling then on Promise');

    greetingPoster.then(res => console.log(`PROMISE 3: Greeting from promise: ${res}`));

    console.log('-------------------------------------------------------');
    const greetingLady$ = new Observable(observer => {
      console.log('Inside Observable (proof of being lazy)');
      observer.next('Hello! I am glad to get to know you.');
      observer.complete();
    });

    console.log('Before calling subscribe on Observable');

    greetingLady$.subscribe({
      next: console.log,
      complete: () => console.log('End of conversation with pretty lady')
    });
  }
}
