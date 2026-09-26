import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { ElectionService } from '../../../core/services/election';
import { environment } from '../../../../environments/environment';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-election-form',
  styleUrl: './election-form.scss',
  templateUrl: './election-form.html'
})
export class ElectionForm implements OnInit {
  private fb = inject(FormBuilder);
  private electionService = inject(ElectionService);
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  isEditMode = signal(false);
  electionId = signal<number | null>(null);
  errorMessage = signal('');
  isSubmitting = signal(false);
  departments = signal<{ id: number; name: string }[]>([]);

  form = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    departmentId: [null as number | null, Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    minTenureYears: [0, [Validators.required, Validators.min(0)]]
  });

  async ngOnInit(): Promise<void> {
    const data = await firstValueFrom(
      this.http.get<{ id: number; name: string }[]>(`${environment.apiUrl}/departments`)
    );
    this.departments.set(data);

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode.set(true);
      this.electionId.set(Number(idParam));
      const election = await this.electionService.getById(Number(idParam));
      this.form.patchValue({
        title: election.title,
        description: election.description,
        departmentId: election.departmentId,
        startDate: election.startDate.substring(0, 16),
        endDate: election.endDate.substring(0, 16),
        minTenureYears: election.minTenureYears
      });
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;

    this.errorMessage.set('');
    this.isSubmitting.set(true);

    try {
      const request = this.form.getRawValue() as any;
      if (this.isEditMode()) {
        await this.electionService.update(this.electionId()!, request);
        this.router.navigate(['/elections', this.electionId()]);
      } else {
        const created = await this.electionService.create(request);
        this.router.navigate(['/elections', created.id]);
      }
    } catch (err: any) {
      this.errorMessage.set(err.error?.message ?? 'Failed to save election.');
    } finally {
      this.isSubmitting.set(false);
    }
  }
}