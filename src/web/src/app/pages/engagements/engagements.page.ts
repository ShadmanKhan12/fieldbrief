import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { EngagementListItem } from '../../core/api.models';
import { ApiService } from '../../core/api.service';

@Component({
  selector: 'app-engagements-page',
  imports: [RouterLink, DatePipe],
  templateUrl: './engagements.page.html',
  styleUrl: './engagements.page.scss'
})
export class EngagementsPage implements OnInit {
  private readonly api = inject(ApiService);

  readonly items = signal<EngagementListItem[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.listEngagements().subscribe({
      next: (items) => {
        this.items.set(items);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.message ?? 'Failed to load engagements');
        this.loading.set(false);
      }
    });
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Active':
        return 'success';
      case 'Blocked':
        return 'error';
      case 'Discovery':
        return 'warning';
      default:
        return 'neutral';
    }
  }
}
