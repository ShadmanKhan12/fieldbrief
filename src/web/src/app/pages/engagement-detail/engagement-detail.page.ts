import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EngagementDetail } from '../../core/api.models';
import { ApiService } from '../../core/api.service';

@Component({
  selector: 'app-engagement-detail-page',
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './engagement-detail.page.html',
  styleUrl: './engagement-detail.page.scss'
})
export class EngagementDetailPage implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);

  readonly engagement = signal<EngagementDetail | null>(null);
  readonly loading = signal(false);
  readonly running = signal(false);
  readonly message = signal<string | null>(null);

  decisionTitle = '';
  decisionContext = '';
  decisionText = '';
  decisionConsequences = '';
  feedBackToProduct = false;
  noteBody = '';

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }
    this.loading.set(true);
    this.api.getEngagement(id).subscribe({
      next: (e) => {
        this.engagement.set(e);
        this.loading.set(false);
      },
      error: () => {
        this.message.set('Failed to load engagement.');
        this.loading.set(false);
      }
    });
  }

  runAllChecks(): void {
    const e = this.engagement();
    if (!e) {
      return;
    }
    this.running.set(true);
    this.message.set(null);
    this.api.runChecks(e.id).subscribe({
      next: () => {
        this.running.set(false);
        this.message.set('Health checks finished.');
        this.reload();
      },
      error: () => {
        this.running.set(false);
        this.message.set('Health check run failed.');
      }
    });
  }

  runOne(checkId: string): void {
    this.api.runCheck(checkId).subscribe({
      next: () => this.reload(),
      error: () => this.message.set('Single check failed.')
    });
  }

  toggleItem(itemId: string, isDone: boolean): void {
    const e = this.engagement();
    if (!e) {
      return;
    }
    this.api.toggleChecklist(e.id, itemId, isDone).subscribe({
      next: () => this.reload(),
      error: () => this.message.set('Could not update checklist.')
    });
  }

  addDecision(): void {
    const e = this.engagement();
    if (!e || !this.decisionTitle.trim() || !this.decisionText.trim()) {
      return;
    }
    this.api
      .createDecision(e.id, {
        title: this.decisionTitle,
        context: this.decisionContext,
        decision: this.decisionText,
        consequences: this.decisionConsequences,
        feedBackToProduct: this.feedBackToProduct
      })
      .subscribe({
        next: () => {
          this.decisionTitle = '';
          this.decisionContext = '';
          this.decisionText = '';
          this.decisionConsequences = '';
          this.feedBackToProduct = false;
          this.reload();
        },
        error: () => this.message.set('Could not save decision.')
      });
  }

  addNote(): void {
    const e = this.engagement();
    if (!e || !this.noteBody.trim()) {
      return;
    }
    this.api.createNote(e.id, { body: this.noteBody, author: 'FDE' }).subscribe({
      next: () => {
        this.noteBody = '';
        this.reload();
      },
      error: () => this.message.set('Could not save note.')
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
