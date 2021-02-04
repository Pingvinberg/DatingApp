import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private membersService: MembersService, private route: ActivatedRoute, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.loadMember();

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]    
  }  

  loadMember()
  {
    this.membersService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member =>
      {
        this.member = member;
        this.galleryImages = this.getImages();
      })      
  }
  
  getImages(): NgxGalleryImage[] 
  {
    const imageUrls = [];
    for(const photo of this.member.photos)
    {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url        
      })
    }
    return imageUrls;
  }

  addLike(member: Member)
  {
    this.membersService.addLike(member.username).subscribe(() => 
    {
      this.toastr.success("You have liked " + member.username);
    })
  }

  removeLike(member: Member)
  {
    this.membersService.removeLike(member.id).subscribe(() => 
    {
      this.toastr.warning("You have removed your like on " + member.username);
    })
  }
}
