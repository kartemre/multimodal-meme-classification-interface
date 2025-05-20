import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
  displayedColumns: string[] = ['id', 'reporter', 'reportedContent', 'reason', 'status', 'createdAt', 'actions'];
  dataSource: MatTableDataSource<any>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    // Initialize with sample data
    const reports = [
      {
        id: 1,
        reporter: 'user1',
        reportedContent: 'Post #123',
        reason: 'inappropriate',
        status: 'pending',
        createdAt: '2024-02-20'
      },
      {
        id: 2,
        reporter: 'user2',
        reportedContent: 'Comment #456',
        reason: 'spam',
        status: 'under_review',
        createdAt: '2024-02-19'
      }
    ];

    this.dataSource = new MatTableDataSource(reports);
  }

  ngOnInit(): void {}

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  getReasonColor(reason: string): string {
    switch (reason) {
      case 'inappropriate':
        return 'warn';
      case 'spam':
        return 'accent';
      case 'harassment':
        return 'primary';
      default:
        return 'primary';
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'pending':
        return 'warn';
      case 'under_review':
        return 'accent';
      case 'resolved':
        return 'primary';
      default:
        return 'primary';
    }
  }

  viewReport(row: any) {
    // Implement view report logic
    console.log('View report:', row);
  }

  resolveReport(row: any) {
    // Implement resolve report logic
    console.log('Resolve report:', row);
  }

  deleteReport(row: any) {
    // Implement delete report logic
    console.log('Delete report:', row);
  }

  updateReportStatus(report: any, status: string) {
    // Implement update report status logic
    console.log('Update report status:', report, status);
  }
} 