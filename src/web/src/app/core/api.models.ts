export interface EngagementListItem {
  id: string;
  code: string;
  customerName: string;
  title: string;
  status: string;
  owner: string;
  updatedAt: string;
  systemCount: number;
  openChecklistCount: number;
}

export interface HealthCheckResult {
  id: string;
  ranAt: string;
  passed: boolean;
  statusCode: number | null;
  durationMs: number;
  error: string | null;
}

export interface HealthCheck {
  id: string;
  name: string;
  method: string;
  path: string;
  expectedStatusCode: number;
  lastResult: HealthCheckResult | null;
}

export interface CustomerSystem {
  id: string;
  name: string;
  kind: string;
  baseUrl: string;
  isCritical: boolean;
  notes: string | null;
  healthChecks: HealthCheck[];
}

export interface Decision {
  id: string;
  title: string;
  context: string;
  decision: string;
  consequences: string;
  madeBy: string;
  madeAt: string;
  feedBackToProduct: boolean;
}

export interface ChecklistItem {
  id: string;
  title: string;
  phase: string;
  isDone: boolean;
  sortOrder: number;
  completedAt: string | null;
}

export interface Note {
  id: string;
  author: string;
  body: string;
  createdAt: string;
}

export interface EngagementDetail {
  id: string;
  code: string;
  customerName: string;
  title: string;
  problemStatement: string;
  desiredOutcome: string;
  status: string;
  owner: string;
  startedAt: string;
  updatedAt: string;
  systems: CustomerSystem[];
  decisions: Decision[];
  checklist: ChecklistItem[];
  notes: Note[];
}
