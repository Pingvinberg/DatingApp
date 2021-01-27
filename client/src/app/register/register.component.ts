import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;

  constructor(private accountService: AccountService, private toastr: ToastrService, private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm()
  }

  initializeForm()
  {
    this.registerForm = this.fb.group({
      gender: ['male'],      
      username: ['', [Validators.required, Validators.minLength(4)]],
      knowAs: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['' , [Validators.required, this.matchValue('password')]],
      city: ['', [Validators.required]],
      country: ['', [Validators.required]],
    }, /*this.passwordMatch*/)    
  }

  matchValue(matchTo: string): ValidatorFn
  {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching : true}
    }
  }
  /*passwordMatch(registerForm: FormGroup)
  {
    return registerForm.get('password').value === registerForm.get('confirmPassword').value ? null : {'mismatch': true}
  }*/

  register() {    
    this.accountService.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl('/members');
    }, error => {
      console.log(error);    
    })
  }

  cancel()
  {
    this.cancelRegister.emit(false);
  }

}
