import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import {
  Decision,
  EngagementDetail,
  EngagementListItem,
  HealthCheckResult,
  Note
} from './api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);

  listEngagements() {
    return this.http.get<EngagementListItem[]>('/api/v1/engagements');
  }

  getEngagement(id: string) {
    return this.http.get<EngagementDetail>(`/api/v1/engagements/${id}`);
  }

  createDecision(
    engagementId: string,
    body: {
      title: string;
      context: string;
      decision: string;
      consequences: string;
      feedBackToProduct: boolean;
    }
  ) {
    return this.http.post<Decision>(`/api/v1/engagements/${engagementId}/decisions`, body);
  }

  createNote(engagementId: string, body: { body: string; author?: string }) {
    return this.http.post<Note>(`/api/v1/engagements/${engagementId}/notes`, body);
  }

  toggleChecklist(engagementId: string, itemId: string, isDone: boolean) {
    return this.http.patch<void>(`/api/v1/engagements/${engagementId}/checklist/${itemId}`, {
      isDone
    });
  }

  runChecks(engagementId: string) {
    return this.http.post<HealthCheckResult[]>(`/api/v1/engagements/${engagementId}/checks/run`, {});
  }

  runCheck(checkId: string) {
    return this.http.post<HealthCheckResult>(`/api/v1/engagements/checks/${checkId}/run`, {});
  }
}
