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
            '#36A2EB', // Синий
            '#FF6384', // Розовый
            '#FFCE56', // Желтый
            '#4BC0C0', // Бирюзовый
            '#9966FF', // Фиолетовый
            '#FF9F40', // Оранжевый
            '#FF5733', // Красный
            '#C70039', // Темно-красный
            '#900C3F', // Бордовый
            '#581845', // Темно-бордовый
            '#DAF7A6', // Светло-зеленый
            '#FFC300', // Золотой
            '#FF5733', // Оранжево-красный
            '#C70039', // Ярко-красный
            '#900C3F', // Темно-бордовый
            '#581845', // Темно-фиолетовый
            '#FFC300', // Светло-желтый
            '#DAF7A6', // Светло-зеленый
            '#FF6F61', // Персиковый
            '#6B5B95', // Сине-фиолетовый
            '#88B04B'  // Зеленый
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
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            //navigate('/404');
        }
    }
}

export default AnalyticsAllUserSurvey;