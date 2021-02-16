import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery';
import { ToastrService } from 'ngx-toastr';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { Input } from '@angular/core';
import { Message } from 'src/app/_modules/message';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;  
  messages: Message[] = [];


  constructor(private membersService: MembersService, private route: ActivatedRoute, private toastr: ToastrService, private messagesService: MessageService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;      
    })

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

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
    this.galleryImages = this.getImages();   
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

  onTabActivaded(data: TabDirective)
  {
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages' || this.messages.length === 0) {
      this.loadMessages();
    }
  }

  loadMessages()
  {
    this.messagesService.getMessageThread(this.member.username).subscribe(messages => { 
      this.messages = messages;
     })
  }

  selectTab(tabId: number)
  {
    this.memberTabs.tabs[tabId].active = true;
  }
}
