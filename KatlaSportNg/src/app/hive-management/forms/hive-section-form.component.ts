import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveService } from '../services/hive.service';
import { HiveSection } from '../models/hive-section';
import { HiveSectionService } from '../services/hive-section.service';

@Component({
  selector: 'app-hive-section-form',
  templateUrl: './hive-section-form.component.html',
  styleUrls: ['./hive-section-form.component.css']
})
export class HiveSectionFormComponent implements OnInit {
  hiveId: number;
  hiveSection: HiveSection = new HiveSection(0, "", "", false, "");
  existed = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveSectionService: HiveSectionService
  ) { }

  ngOnInit() {
    this.route.params.subscribe(p => {
      this.hiveId = p['hiveId'];
      if (p['hiveSectionId'] === undefined) return;
      this.hiveSectionService.getHiveSection(p['hiveSectionId']).subscribe(s => this.hiveSection = s);      
      this.existed = true;
    });
  }

  navigateToHiveSections() {
    this.router.navigate([`/hive/${this.hiveId}/sections`]);
  }

  onCancel() {
    this.navigateToHiveSections();
  }
  
  onSubmit() {
    if (this.existed) {
      this.hiveSectionService.updateHiveSection(this.hiveSection)
        .subscribe(
          resp => {
            this.navigateToHiveSections();
          }
        );
    }
    else {
      this.hiveSectionService.addHiveSection(this.hiveId, this.hiveSection)
        .subscribe(
          resp => {
            this.navigateToHiveSections();
          }
        );
    }
  }

  onDelete() {
    this.setStatus(true);
  }

  onUndelete() {
    this.setStatus(false);
  }

  onPurge() {
    this.hiveSectionService.deleteHiveSection(this.hiveSection.id).subscribe(h => this.navigateToHiveSections());
  }

  private setStatus(newStatus: boolean) {
    this.hiveSectionService.setHiveSectionStatus(this.hiveSection.id, newStatus).subscribe(o => this.hiveSection.isDeleted = newStatus);
  }
}
