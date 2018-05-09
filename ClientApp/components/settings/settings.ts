 import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import VueRouter from 'vue-router';



@Component
export default class SettingsComponent extends Vue {
    $http: any;
    settings: any = {};


    GetSettings() {
        this.$http.get('Settings/All', {
        }).then((response: any) => {
            this.settings = response.data;

            if (this.settings.mailSubscribers == null)
                this.settings.mailSubscribers = [];

        }).catch((error: any) => {
            alert(error);
        });
    };

    SaveSettings() {
        this.$http.post('Settings/Save', {
            BackupDirectory: this.settings.backupDirectory,
            TaskEvaluationInterval: this.settings.taskEvaluationInterval,
            MailSubscribers: this.settings.mailSubscribers,
            MailReminderInterval: this.settings.mailReminderInterval
        }).then((response: any) => {

        }).catch((error: any) => {
            alert(error);
        });
    }

    removeMail(item: string) {
        this.settings.subscribers.splice(this.settings.mailSubscribers.indexOf(item), 1)
        this.settings.subscribers = [...this.settings.mailSubscribers]
    }

    mounted() {
        this.GetSettings();
    }
}
