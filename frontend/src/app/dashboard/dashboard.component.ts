import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { StateService } from '../services/state.service';
import { RealEstateList } from '../models/real-estate';
import { RealEstateService } from '../services/real-estate-management.service';
import { States } from '../models/state';
import { Router } from '@angular/router';
import { SessionStorageService } from '../services/session-storage.service';
import { catchError, finalize, throwError } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  realEstates$!: RealEstateList;
  loading = false;
  searchError = false;
  searchErrorMsg!: string;
  states!: States;
  cities!: string[];
  page!: number;
  notAuthorizedMsg = "Not authorized, return to login page!";
  showInfo = false;

  stateControl!: FormControl;
  cityControl!: FormControl;
  saleModeControl!: FormControl;
  recordsPerPageControl!: FormControl;

  selectedFile: File | null = null;
  isUploading = false;
  uploadError: string | null = null;
  uploadSuccess = false;

  constructor(private fb: FormBuilder,
    private statesService: StateService,
    private realEstateService: RealEstateService,
    private sessionStorageService: SessionStorageService,
    private router: Router) { }

  async ngOnInit() {
    this.loading = true;

    await this.realEstateService.get(0, 10).subscribe(x => {
      this.realEstates$ = x!.result;
    });

    await this.statesService.getStates().then(x => {
      this.states = x;
    })

    this.initializeFormControls();
    this.loading = false;

    this.stateControl.valueChanges.subscribe(selectedState => {
      this.updateCities(selectedState);
      this.page = 0;
    });

    this.recordsPerPageControl.valueChanges.subscribe(_ => {
      this.search();
    });
  }

  initializeFormControls() {
    this.stateControl = this.fb.control('ALL');
    this.states.forEach(x => {
      this.stateControl.setValue(x.uf);
    });
    this.stateControl.setValue('ALL', { emitEvent: false });
    this.cityControl = this.fb.control('ALL');
    this.saleModeControl = this.fb.control('ALL');
    this.recordsPerPageControl = this.fb.control(10);
    this.page = 0;
  }

  updateCities(selectedState: string) {
    const state = this.states.find(s => s.uf === selectedState);

    if (state)
      this.cities = state.cities || [];
    else
      this.cities = [];

    this.cityControl.setValue('ALL');
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];

    // Validate file type
    if (file && file.type === 'text/csv') {
      this.selectedFile = file;
      this.uploadError = null;
      this.uploadSuccess = false;
    } else {
      this.selectedFile = null;
      this.uploadError = 'Please select a valid CSV file.';
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.uploadError = 'No file selected';
      return;
    }

    const formData = new FormData();
    formData.append('file', this.selectedFile, this.selectedFile.name);

    this.isUploading = true;
    this.uploadError = null;
    this.uploadSuccess = false;
    this.realEstateService.bulkAdd(formData)
      .pipe(
        catchError(error => {
          this.uploadError = 'Upload failed';
          if (error.status === 401)
            this.uploadError = this.notAuthorizedMsg;
          return throwError(() => new Error(error));
        }),
        finalize(() => {
          this.isUploading = false;
        })
      )
      .subscribe({
        next: () => {
          this.uploadSuccess = true;
        }
      });
  }

  toggleInfo() {
    this.showInfo = !this.showInfo;
  }

  updatePageNumber(pageChange: number = 0) {
    this.page += pageChange;
    if (this.page < 0)
      this.page = 0;

    this.search();
  }

  async search() {
    this.searchError = false;
    this.loading = true;

    const state = this.stateControl.value !== 'ALL' ? this.stateControl.value : undefined;
    const city = this.cityControl.value !== 'ALL' ? this.cityControl.value : undefined;
    const saleMode = this.saleModeControl.value !== 'ALL' ? this.saleModeControl.value : undefined;

    await this.realEstateService.get(this.page, this.recordsPerPageControl.value, state, city, saleMode)
      .pipe(
        catchError(error => {

          this.realEstates$ = [];
          this.searchError = true;
          if (error.status === 401)
            this.searchErrorMsg = this.notAuthorizedMsg;
          else if(error.status === 404)
            this.searchErrorMsg = 'Real Estates were not found!';
          else
            this.searchErrorMsg = 'Internal Server Error!';

          return throwError(() => new Error(error));
        }),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (x) => {
          this.realEstates$ = x!.result;
        }
      });
  }

  logout() {
    this.sessionStorageService.deleteTokenSessionStorage();
    this.router.navigate(['']);
  }
}
