import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;

  constructor(private http: HttpClient) { }

  /**
   * A callback method that is invoked immediately after the default change detector has checked
   * the directive's data-bound properties for the first time,
   * and before any of the view or content children have been checked.
   * It is invoked only once when the directive is instantiated.
   */
  ngOnInit() {
  }

  /**
   * Registration content switcher.
   */
  registerToggle() {
    this.registerMode = true;
  }

  /**
   * Cancels register mode
   * @param registerMode Boolean for registration content switcher.
   */
  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
