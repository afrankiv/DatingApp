import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

/**
 * ANGULAR -- OnInit --
 * A lifecycle hook that is called after Angular has initialized all data-bound properties of a directive.
 * Define an ngOnInit() method to handle any additional initialization tasks.
 *
 * COMPONENT
 * 1. Represents logic to display collection of users including server side paging, sorting and filtering.
 * 2.
 */
@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // Users(members) collection
  users: User[];

  // LOCAL STORAGE: used to store login user information.
  user: User = JSON.parse(localStorage.getItem('user'));

  // UI list for filtering
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' }
  ];

  // User params represents client-server request model for paging, filtering and sorting.
  userParams: any = {};
  // Pagination model will be transferred between client-server through http headers.
  pagination: Pagination;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  /**
   * A callback method that is invoked immediately after the default change detector has checked
   * the directive's data-bound properties for the first time,
   * and before any of the view or content children have been checked.
   * It is invoked only once when the directive is instantiated.
   */
  ngOnInit() {
    // ANGULAR -- ActivatedRoute -- route.data: An observable of the static and resolved data of this route.
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    // Setup default filtering parameters
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
  }

  /**
   * NGX Bootstrap pagination event handler
   * @param event ngx pagination event
   */
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  /**
   * Resets users list to default filtering parameters
   */
  resetFilters() {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers();
  }

  /**
   * Calls backend to load users based on selected page number.
   */
  loadUsers() {
    this.userService
      // returns Observable<PaginatedResult<User[]>>
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe(
        (res: PaginatedResult<User[]>) => {
          this.users = res.result;
          this.pagination = res.pagination;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
