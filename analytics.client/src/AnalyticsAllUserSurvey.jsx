import { useEffect, useState } from 'react';
//import './AnalyticAllUserTest.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';
import { Pie } from 'react-chartjs-2';
import 'chart.js/auto';
import { AddButtonListEmployees, AddButtonUserProfile, AddButtonHRPanel, AddButtonAdminPanel } from './funcs/Buttons';

function AnalyticsAllUserSurvey() {
    const { id } = useParams();
    const [chartsArr, setCharArr] = useState([]);

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        getAnalytics();
    }, []);

    return (
        <>
            <div className="container">
                <div className="profile-buttons">
                    {AddButtonListEmployees(navigate)}
                    {AddButtonHRPanel(navigate)}
                    {AddButtonAdminPanel(navigate)}
                    {AddButtonUserProfile(navigate)}
                </div>
            </div>
            {chartsArr ? (
                <>
                    {chartsArr.map(chart => (
                        <div className="chart-container">
                            <h3>{chart.title}</h3>
                            <div className="chart">
                                <Pie
                                    data={chart.data}
                                    options={{
                                        plugins: { legend: { display: false } },
                                    }}
                                />
                            </div>
                        </div>
                    ))}
                </>
            ) : null}

        </>
    );

    async function getAnalytics() {
        const colors = [
            '#36A2EB', // �����
            '#FF6384', // �������
            '#FFCE56', // ������
            '#4BC0C0', // ���������
            '#9966FF', // ����������
            '#FF9F40', // ���������
            '#FF5733', // �������
            '#C70039', // �����-�������
            '#900C3F', // ��������
            '#581845', // �����-��������
            '#DAF7A6', // ������-�������
            '#FFC300', // �������
            '#FF5733', // ��������-�������
            '#C70039', // ����-�������
            '#900C3F', // �����-��������
            '#581845', // �����-����������
            '#FFC300', // ������-������
            '#DAF7A6', // ������-�������
            '#FF6F61', // ����������
            '#6B5B95', // ����-����������
            '#88B04B'  // �������
        ];

        try {
            const response = await fetch(`analyticsusersurvey/all?surveyId=${id}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (response.ok) {
                const result = await response.json();

                let tmpArr = []

                Object.entries(result).map(([question, data]) => {
                    const title = question;
                    const chartData = {
                        labels: Object.keys(data.data),
                        datasets: [
                            {
                                data: Object.values(data.data),
                                backgroundColor: colors,
                            }
                        ]
                    };
                    tmpArr = [...tmpArr, { title: title, data: chartData }]
                });


                setCharArr(tmpArr)
            }
            if (response.status === 401) {
                console.error('������ ��� ��������� ������:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('������ ��� ��������:', error);
            //navigate('/404');
        }
    }
}

export default AnalyticsAllUserSurvey;