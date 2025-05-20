import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  generalSettingsForm: FormGroup;
  notificationSettingsForm: FormGroup;
  securitySettingsForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.generalSettingsForm = this.fb.group({
      siteName: ['My Application', Validators.required],
      siteDescription: ['A modern web application', Validators.required],
      maintenanceMode: [false],
      allowRegistration: [true],
      defaultLanguage: ['en', Validators.required]
    });

    this.notificationSettingsForm = this.fb.group({
      emailNotifications: [true],
      pushNotifications: [true],
      notificationSound: [true],
      notificationEmail: ['admin@example.com', [Validators.required, Validators.email]]
    });

    this.securitySettingsForm = this.fb.group({
      twoFactorAuth: [false],
      sessionTimeout: [30, [Validators.required, Validators.min(5), Validators.max(120)]],
      passwordExpiry: [90, [Validators.required, Validators.min(30), Validators.max(365)]],
      maxLoginAttempts: [5, [Validators.required, Validators.min(3), Validators.max(10)]]
    });
  }

  ngOnInit(): void {
  }

  saveGeneralSettings() {
    if (this.generalSettingsForm.valid) {
      console.log('Saving general settings:', this.generalSettingsForm.value);
      this.showSuccessMessage('General settings saved successfully');
    }
  }

  saveNotificationSettings() {
    if (this.notificationSettingsForm.valid) {
      console.log('Saving notification settings:', this.notificationSettingsForm.value);
      this.showSuccessMessage('Notification settings saved successfully');
    }
  }

  saveSecuritySettings() {
    if (this.securitySettingsForm.valid) {
      console.log('Saving security settings:', this.securitySettingsForm.value);
      this.showSuccessMessage('Security settings saved successfully');
    }
  }

  private showSuccessMessage(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }
} 