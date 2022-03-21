import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../#services/user.service';
import { User } from '../../../#models/user.model';
import { AlertifyService } from '../../../#services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/#models/pagination.model';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'Witch', display: 'Witches'}, {value: 'Wizard', display: 'Wizards'}, {value: 'All', display: 'All'}];
  userParams: any = {};
  pagination: Pagination;

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.userParams.gender = 'All';
    this.userParams.minAge = 0;
    this.userParams.maxAge = 999;
    this.userParams.orderBy = 'LastActive';
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  resetFilters() {
    this.userParams.gender = 'All';
    this.userParams.minAge = 0;
    this.userParams.maxAge = 999;
    this.loadUsers();
  }

  loadUsers() {
    this.userService
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
