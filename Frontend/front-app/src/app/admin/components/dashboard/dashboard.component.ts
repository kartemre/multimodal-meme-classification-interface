import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  stats = {
    totalUsers: 150,
    totalPosts: 450,
    totalReports: 25,
    activeUsers: 120
  };

  recentActivities = [
    { type: 'user', action: 'New user registered', time: '5 minutes ago' },
    { type: 'post', action: 'New post created', time: '15 minutes ago' },
    { type: 'report', action: 'New report submitted', time: '30 minutes ago' },
    { type: 'user', action: 'User profile updated', time: '1 hour ago' }
  ];

  constructor() {}

  ngOnInit(): void {}
} 