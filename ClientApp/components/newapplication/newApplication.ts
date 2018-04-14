import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';

interface Application {
    ID: string;
    Name: string;
    Description: string;
    CreationDate: number;
    Tasks: any;
}


@Component({
    components: {
        MenuComponent: require('../navmenu/navmenu.vue.html')
    }
})

export default class NewApplicationComponent extends Vue {
    Name: string = "";
    Description: string = "";
    IsCreating: boolean = false;
    $route: any;
    $http: any;

    //Notification
    notification: boolean = false;
    notificationText: string = "";
    notificationColor: string = "success";

    errorMessage: string = "";

    @Prop({ default: false })
    value!: boolean;

    hideCreateApplicationDialog() {
        this.$parent.$data.ShowCreateApplicationDialog = false;
    }

    clear() {
        this.Name = "";
        this.Description = "";
    }

    createApplication() {
        this.$http.post('Application/Create', {
            Name: this.Name,
            Description: this.Description
        }).then((response: any) => {
            this.hideCreateApplicationDialog();
            this.$emit('created');
            this.IsCreating = false;

            this.notificationText = "Application created!"
            this.notification = true;
            this.notificationColor = "success";
        }).catch((error: any) => {
            this.errorMessage = error.response.data;
            this.IsCreating = false;
            });

        this.IsCreating = true;
    }
}
