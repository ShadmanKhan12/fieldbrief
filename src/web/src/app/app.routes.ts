import { Routes } from '@angular/router';
import { EngagementDetailPage } from './pages/engagement-detail/engagement-detail.page';
import { EngagementsPage } from './pages/engagements/engagements.page';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'engagements' },
  { path: 'engagements', component: EngagementsPage },
  { path: 'engagements/:id', component: EngagementDetailPage },
  { path: '**', redirectTo: 'engagements' }
];
